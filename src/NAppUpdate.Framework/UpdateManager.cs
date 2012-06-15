using System.Collections.Generic;
using System.IO;
using System;
using System.Threading;
using NAppUpdate.Framework.Common;
using NAppUpdate.Framework.FeedReaders;
using NAppUpdate.Framework.Sources;
using NAppUpdate.Framework.Tasks;

namespace NAppUpdate.Framework
{
	/// <summary>
	/// An UpdateManager class is a singleton class handling the update process from start to end for a consumer application
	/// </summary>
	public sealed class UpdateManager
	{
		#region Singleton Stuff

		/// <summary>
		/// Defaut ctor
		/// </summary>
		private UpdateManager()
		{
			State = UpdateProcessState.NotChecked;
			UpdatesToApply = new List<IUpdateTask>();
			ApplicationPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
			Config = new NauConfigurations
			         	{
			         		TempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()),
			         		UpdateProcessName = "NAppUpdateProcess",
			         		UpdateExecutableName = "foo.exe", // Naming it updater.exe seem to trigger the UAC, and we don't want that
			         	};

			// Need to do this manually here because the BackupFolder property is protected using the static instance, which we are
			// in the middle of creating
			string backupPath = Path.Combine(Path.GetDirectoryName(ApplicationPath) ?? string.Empty, "Backup" + DateTime.Now.Ticks);
			backupPath = backupPath.TrimEnd(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			Config._backupFolder = Path.IsPathRooted(backupPath) ? backupPath : Path.Combine(Config.TempFolder, backupPath);
		}

		static UpdateManager() {}

		/// <summary>
		/// The singleton update manager instance to used by consumer applications
		/// </summary>
		public static UpdateManager Instance
		{
			get { return instance; }
		}
		private static readonly UpdateManager instance = new UpdateManager();

		#endregion

		/// <summary>
		/// State of the update process
		/// </summary>
		public enum UpdateProcessState
		{
			NotChecked,
			Checked,
			Prepared,
			AppliedSuccessfully,
			RollbackRequired,
		}

		internal readonly string ApplicationPath;

		public NauConfigurations Config { get; set; }

		internal string BaseUrl { get; set; }
		public IList<IUpdateTask> UpdatesToApply { get; private set; }
		public int UpdatesAvailable { get { return UpdatesToApply == null ? 0 : UpdatesToApply.Count; } }
		public UpdateProcessState State { get; private set; }
		public string LatestError { get; set; }

		public IUpdateSource UpdateSource { get; set; }
		public IUpdateFeedReader UpdateFeedReader { get; set; }

		private Thread _updatesThread;
		internal volatile bool ShouldStop;
		public bool IsWorking { get { return _updatesThread != null && _updatesThread.IsAlive; } }

		#region Progress reporting

		public event ReportProgressDelegate ReportProgress;
		private void TaskProgressCallback(UpdateProgressInfo currentStatus, IUpdateTask task)
		{
			if (ReportProgress == null) return;

			currentStatus.TaskDescription = task.Description;
			currentStatus.TaskId = UpdatesToApply.IndexOf(task) + 1;

			var taskPerc = 100/UpdatesToApply.Count;
			currentStatus.Percentage = (currentStatus.Percentage * taskPerc / 100) + (currentStatus.TaskId - 1) * taskPerc;

			ReportProgress(currentStatus);
		}

		#endregion

		#region Step 1 - Check for updates

		/// <summary>
		/// Check for update synchronously, using the default update source
		/// </summary>
		/// <returns>true if successful and updates exist</returns>
		public bool CheckForUpdates()
		{
			return CheckForUpdates(UpdateSource, null);
		}

		/// <summary>
		/// Check for updates synchronously
		/// </summary>
		/// <param name="source">An update source to use</param>
		/// <returns>true if successful and updates exist</returns>
		public bool CheckForUpdates(IUpdateSource source)
		{
			return CheckForUpdates(source ?? UpdateSource, null);
		}

		/// <summary>
		/// Check for updates synchronouly
		/// </summary>
		/// <param name="source">Updates source to use</param>
		/// <param name="countCallback">Callback function to call when done</param>
		/// <returns>true if successful and updates exist</returns>
		private bool CheckForUpdates(IUpdateSource source, Action<int> countCallback)
		{
			LatestError = null;

			if (UpdateFeedReader == null)
				throw new ArgumentException("An update feed reader is required; please set one before checking for updates");

			if (source == null)
				throw new ArgumentException("An update source was not specified");

			lock (UpdatesToApply)
			{
				UpdatesToApply.Clear();
				var tasks = UpdateFeedReader.Read(source.GetUpdatesFeed());
				foreach (var t in tasks)
				{
					if (ShouldStop) return false;
					if (t.UpdateConditions.IsMet(t)) // Only execute if all conditions are met
						UpdatesToApply.Add(t);
				}
			}

			if (ShouldStop) return false;

			State = UpdateProcessState.Checked;
			if (countCallback != null) countCallback(UpdatesToApply.Count);

			if (UpdatesToApply.Count > 0)
				return true;

			return false;
		}

		/// <summary>
		/// Check for updates asynchronously
		/// </summary>
		/// <param name="callback">Callback function to call when done</param>
		public void CheckForUpdateAsync(Action<int> callback)
		{
			CheckForUpdateAsync(UpdateSource, callback);
		}

		/// <summary>
		/// Check for updates asynchronously
		/// </summary>
		/// <param name="source">Update source to use</param>
		/// <param name="countCallback">Callback function to call when done</param>
		private void CheckForUpdateAsync(IUpdateSource source, Action<int> countCallback)
		{
			if (IsWorking) return;

			_updatesThread = new Thread(delegate()
											{
												try
												{
													CheckForUpdates(source, countCallback);
												}
												catch (Exception ex)
												{
													// TODO: Better error handling
													LatestError = ex.ToString();
													if (countCallback != null) countCallback(-1);
												}
											}) { IsBackground = true };
			_updatesThread.Start();
		}

		#endregion

		#region Step 2 - Prepare to execute update tasks

		/// <summary>
		/// Prepare updates synchronously
		/// </summary>
		/// <returns>true if successful</returns>
		public bool PrepareUpdates()
		{
			return PrepareUpdates(null);
		}

		/// <summary>
		/// Prepare updates synchronously, calling the provided callback when done
		/// </summary>
		/// <param name="callback">A callback function to execute when done</param>
		/// <returns>true if successful</returns>
		private bool PrepareUpdates(Action<bool> callback)
		{
			LatestError = null;

			lock (UpdatesToApply)
			{
				if (UpdatesToApply.Count == 0)
				{
					if (callback != null) callback(false);
					return false;
				}

				if (!Directory.Exists(Config.TempFolder))
					Directory.CreateDirectory(Config.TempFolder);

				foreach (var task in UpdatesToApply)
				{
					if (ShouldStop)
						return false;

					var t = task;
					task.OnProgress += status => TaskProgressCallback(status, t);

					if (!task.Prepare(UpdateSource))
					{
						task.ExecutionStatus = TaskExecutionStatus.FailedToPrepare;
						return false;
					}
				}

				State = UpdateProcessState.Prepared;
			}

			if (ShouldStop) return false;

			if (callback != null) callback(true);
			return true;
		}

		/// <summary>
		/// Prepare updates asynchronously
		/// </summary>
		/// <param name="callback">callback function to call when done</param>
		public void PrepareUpdatesAsync(Action<bool> callback)
		{
			if (IsWorking) return;

			_updatesThread = new Thread(delegate()
											{
												try
												{
													PrepareUpdates(callback);
												}
												catch (Exception ex)
												{
													// TODO: Better error handling
													LatestError = ex.ToString();
													if (callback != null) callback(false);
												}
											}) { IsBackground = true };

			_updatesThread.Start();
		}

		#endregion

		#region Step 3 - Apply updates

		/// <summary>
		/// Starts the updater executable and sends update data to it, and relaunch the caller application as soon as its done
		/// </summary>
		/// <returns>True if successful (unless a restart was required</returns>
		public bool ApplyUpdates()
		{
			return ApplyUpdates(true);
		}

		/// <summary>
		/// Starts the updater executable and sends update data to it
		/// </summary>
		/// <param name="relaunchApplication">true if relaunching the caller application is required; false otherwise</param>
		/// <returns>True if successful (unless a restart was required</returns>
		public bool ApplyUpdates(bool relaunchApplication)
		{
			return ApplyUpdates(relaunchApplication, false, false);
		}

		/// <summary>
		/// Starts the updater executable and sends update data to it
		/// </summary>
		/// <param name="relaunchApplication">true if relaunching the caller application is required; false otherwise</param>
		/// <param name="updaterDoLogging">true if the updater writes to a log file; false otherwise</param>
		/// <param name="updaterShowConsole">true if the updater shows the console window; false otherwise</param>
		/// <returns>True if successful (unless a restart was required</returns>
		public bool ApplyUpdates(bool relaunchApplication, bool updaterDoLogging, bool updaterShowConsole)
		{
			lock (UpdatesToApply)
			{
				LatestError = null;
				bool revertToDefaultBackupPath = true;

				// Set current directory the the application directory
				// this prevents the updater from writing to e.g. c:\windows\system32
				// if the process is started by autorun on windows logon.
				Environment.CurrentDirectory = Path.GetDirectoryName(ApplicationPath);

				// Make sure the current backup folder is accessible for writing from this process
				string backupParentPath = Path.GetDirectoryName(Config.BackupFolder) ?? string.Empty;
				if (Directory.Exists(backupParentPath) && Utils.PermissionsCheck.HaveWritePermissionsForFolder(backupParentPath))
				{
					// Remove old backup folder, in case this same folder was used previously,
					// and it wasn't removed for some reason
					try
					{
						if (Directory.Exists(Config.BackupFolder))
							Utils.FileSystem.DeleteDirectory(Config.BackupFolder);
						revertToDefaultBackupPath = false;
					}
					catch (UnauthorizedAccessException)
					{
					}

					// Attempt to (re-)create the backup folder
					try
					{
						Directory.CreateDirectory(Config.BackupFolder);

						if (!Utils.PermissionsCheck.HaveWritePermissionsForFolder(Config.BackupFolder))
							revertToDefaultBackupPath = true;
					}
					catch (UnauthorizedAccessException)
					{
						// We're having permissions issues with this folder, so we'll attempt
						// using a backup in a default location
						revertToDefaultBackupPath = true;
					}
				}

				if (revertToDefaultBackupPath)
				{
					Config._backupFolder = Path.Combine(
						Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
						Config.UpdateProcessName + "UpdateBackups" + DateTime.UtcNow.Ticks);

					try
					{
						Directory.CreateDirectory(Config.BackupFolder);
					}
					catch (UnauthorizedAccessException ex)
					{
						// We can't backup, so we abort
						LatestError = ex.ToString();
						return false;
					}
				}

				bool runPrivileged = false, hasColdUpdates = false;
				State = UpdateProcessState.RollbackRequired;
				foreach (var task in UpdatesToApply)
				{
					IUpdateTask t = task;
					task.OnProgress += status => TaskProgressCallback(status, t);

					try
					{
						// Execute the task
						task.ExecutionStatus = task.Execute(false);
					}
					catch (Exception ex)
					{
						// TODO: Log error
						task.ExecutionStatus = TaskExecutionStatus.Failed;
					}

					if (task.ExecutionStatus == TaskExecutionStatus.RequiresAppRestart
						|| task.ExecutionStatus == TaskExecutionStatus.RequiresPrivilegedAppRestart)
					{
						// Record that we have cold updates to run, and if required to run any of them privileged
						runPrivileged = runPrivileged || task.ExecutionStatus == TaskExecutionStatus.RequiresPrivilegedAppRestart;
						hasColdUpdates = true;
						continue;
					}

					// We are being quite explicit here - only Successful return values are considered
					// to be Ok (cold updates are already handled above)
					if (task.ExecutionStatus != TaskExecutionStatus.Successful)
					{
						// TODO: notify about task execution failure using exceptions
						return false;
					}
				}

				// If an application restart is required
				if (hasColdUpdates)
				{
					var dto = new UpdateStarter.NauDto
					          	{
					          		Configs = Instance.Config,
					          		Tasks = Instance.UpdatesToApply,
									AppPath = ApplicationPath,
									WorkingDirectory = Environment.CurrentDirectory,
									RelaunchApplication = relaunchApplication,
					          	};

					var updStarter = new UpdateStarter(runPrivileged);
					updStarter.SetOptions(updaterDoLogging, updaterShowConsole);
					bool createdNew;
					using (var _ = new Mutex(true, Config.UpdateProcessName, out createdNew))
					{
						if (updStarter.Start(dto, Config.TempFolder, Config.UpdateProcessName) == null)
							return false;

						Environment.Exit(0);
					}
				}

				State = UpdateProcessState.AppliedSuccessfully;
				UpdatesToApply.Clear();
			}

			return true;
		}

		#endregion

		/// <summary>
		/// Rollback executed updates in case of an update failure
		/// </summary>
		public void RollbackUpdates()
		{
			lock (UpdatesToApply)
			{
				foreach (var task in UpdatesToApply)
				{
					task.Rollback();
				}

				State = UpdateProcessState.NotChecked;
			}
		}

		/// <summary>
		/// Abort update process, cancelling whatever background process currently taking place without waiting for it to complete
		/// </summary>
		public void Abort()
		{
			Abort(false);
		}

		/// <summary>
		/// Abort update process, cancelling whatever background process currently taking place
		/// </summary>
		/// <param name="waitForTermination">If true, blocks the calling thread until the current process terminates</param>
		public void Abort(bool waitForTermination)
		{
			ShouldStop = true;
			if (waitForTermination && _updatesThread != null && _updatesThread.IsAlive)
			{
				_updatesThread.Join(); // TODO perhaps we should support timeout here instead of per process
				_updatesThread = null;
			}
		}

		/// <summary>
		/// Delete the temp folder as a whole and fail silently
		/// </summary>
		public void CleanUp()
		{
			Abort(true);

			lock (UpdatesToApply)
			{
				UpdatesToApply.Clear();
				State = UpdateProcessState.NotChecked;

				try
				{
					if (Directory.Exists(Config.TempFolder))
						Utils.FileSystem.DeleteDirectory(Config.TempFolder);
				}
				catch { }

				try
				{
					if (Directory.Exists(Config.BackupFolder))
						Utils.FileSystem.DeleteDirectory(Config.BackupFolder);
				}
				catch { }
			}
		}
	}
}