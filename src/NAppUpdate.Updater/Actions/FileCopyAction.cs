using System;
using System.IO;

namespace NAppUpdate.Updater.Actions
{
    internal class FileCopyAction : IUpdateAction
    {
        private string source, dest;

        public FileCopyAction(string _source, string _destination)
        {
            this.source = _source;
            this.dest = _destination;
        }

        #region IUpdateAction Members

        public bool Do()
        {
            // First we need to check whether we have write permissions to this folder, as these are separate to delete permissions
            bool canWriteToFolder = NAppUpdate.Framework.Utils.PermissionsCheck.HaveWritePermissionsForFolder(dest);

            // Only proceed if we can write to the dir
            if (canWriteToFolder)
            {
                if (File.Exists(dest))
                {
                    int retries = 5;
                    bool access = false;

                    while (!access && retries > 0)
                    {
                        try
                        {
                            File.Delete(dest);
                            access = true;
                        }
                        catch (System.UnauthorizedAccessException)
                        {
                            // Since we are root this should only happen if the file is locked, so lets sleep & retry
                            retries--;
                            System.Threading.Thread.Sleep(1000);
                        }
                    }

                    if (!access)
                        throw new Exception("Couldn't gain access to " + dest + " to delete it");
                }
                File.Move(source, dest);
            }
            else
            {
                throw new UnauthorizedAccessException("No write permission available on the file");
            }
            return true;
        }
        #endregion
    }
}

