using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Windows;
using leetreveil.AutoUpdate.Framework;
using Path=System.IO.Path;
using System.IO;
using System.Net;

namespace leetreveil.AutoUpdate.SampleApp
{
    /// <summary>
    /// Interaction logic for UpdateWindow.xaml
    /// </summary>
    public partial class UpdateWindow : Window
    {
        private readonly Update _newUpdate;

        private string updaterPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                                  "ltupdater.exe");

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
            try
            {
                UpdateStarter.Start(_newUpdate.FileUrl);
            }
            catch (Exception exception)
            {
                //error downloading or extracting update, notify user
            }
        }
    }

    public static class UpdateStarter
    {
        private static string updaterPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                         "ltupdater.exe");

        public static void Start(string updatePackageUrl)
        {
            var fDownloader = new FileDownloader(updatePackageUrl);

            byte[] fileData = fDownloader.Download();

            ExtractExecutableFromResource(updaterPath);

            //TODO: if the user does not accept elevation prompt we get an error
            Process.Start(updaterPath, String.Format(@"""{0}""", Process.GetCurrentProcess().MainModule.FileName));

            SendUpdatePackageToUpdateExecutable(fileData);

            Application.Current.Shutdown();
        }

        private static void SendUpdatePackageToUpdateExecutable(byte[] fileData)
        {
            var server = new TcpListener(IPAddress.Any, 13001);
            server.Start();

            //wait for the updater to start and attempt to connect
            TcpClient client = server.AcceptTcpClient();

            NetworkStream stream = client.GetStream();

            var fileDataLength = BitConverter.GetBytes(fileData.Length);
            //send the size of the update package as the first 4 bytes
            stream.Write(fileDataLength, 0, 4);
            //send the rest of the file
            stream.Write(fileData, 0, fileData.Length);

            // Shutdown and end connection
            client.Close();
            //stop listening for connections
            server.Stop();
        }

        private static void ExtractExecutableFromResource(string updateExecutablePath)
        {
            //store the updater temporarily in the appdata folder
            using (var writer = new BinaryWriter(File.Open(updateExecutablePath, FileMode.Create)))
                writer.Write(Properties.Resources.ltupdater);
        }
    }
}