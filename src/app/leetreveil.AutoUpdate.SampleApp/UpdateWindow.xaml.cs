using System;
using System.Diagnostics;
using System.Windows;
using leetreveil.AutoUpdate.Core.FileDownload;
using leetreveil.AutoUpdate.Core.UpdateCheck;
using Winterdom.IO.FileMap;
using Path=System.IO.Path;
using System.IO;
using System.Text;

namespace leetreveil.AutoUpdate.SampleApp
{
    /// <summary>
    /// Interaction logic for UpdateWindow.xaml
    /// </summary>
    public partial class UpdateWindow : Window
    {
        private readonly Update _newUpdate;

        public UpdateWindow(Update newUpdate)
        {
            _newUpdate = newUpdate;
            InitializeComponent();

            this.DataContext = this;
        }

        public string UpdateVersion
        {
            get { return _newUpdate.Version.ToString(); }
        }

        private void InstallNow_Click(object sender, RoutedEventArgs e)
        {
           // var fDownloader = new FileDownloader(_newUpdate.FileUrl);

           // var filePathToUpdate = fDownloader.Download();

            //map file into memory using mem mapped files
            try
            {
                var messageBytes = Encoding.ASCII.GetBytes("Hello!");

                MemoryMappedFile map =
                   MemoryMappedFile.Create(MapProtection.PageReadWrite, messageBytes.Length, @"Local\MyMappedFile");

                using (Stream view = map.MapView(MapAccess.FileMapWrite, 0, messageBytes.Length))
                    view.Write(messageBytes, 0, messageBytes.Length);

            }
            catch (FileMapIOException ey)
            {
                Console.WriteLine("Failed Named MMF: " + ey);
            }


           // if (!String.IsNullOrEmpty(filePathToUpdate))
            //{
                ExtractExecutableFromResource();
                StartUpdaterExeAndShutdown();
           // }
           // else
            {
                //TODO: error downloading the update, display message to user
            }

        }


        private string updaterPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                                  "ltupdater.exe");

        private void ExtractExecutableFromResource()
        {
            //store the updater temporarily in the appdata folder
            using (var writer = new BinaryWriter(File.Open(updaterPath, FileMode.Create)))
                writer.Write(Properties.Resources.ltupdater);
        }

        /// <summary>
        /// Starts the update executable, shuts down the current application so the update can start
        /// </summary>
        /// <param name="compressedUpdateFileName">The name of the compressed file that should be extracted and used to update existing files</param>
        private void StartUpdaterExeAndShutdown()
        {
            //TODO: fix null reference
            var thisAppsFilePath = Process.GetCurrentProcess().MainModule.FileName;

            Process.Start(updaterPath,String.Format(@"""{0}""",thisAppsFilePath));

           // Application.Current.Shutdown();
        }
    }
}