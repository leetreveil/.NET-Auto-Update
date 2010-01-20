using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Windows;
using leetreveil.AutoUpdate.Core;
using leetreveil.AutoUpdate.Core.FileDownload;
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
            var fDownloader = new FileDownloader(_newUpdate.FileUrl);

            byte[] fileData = fDownloader.Download();
            byte[] fileDataLength = BitConverter.GetBytes(fileData.Length);

            ExtractExecutableFromResource();

            //TODO: if the user does not accept elevation prompt we get an error
            Process.Start(updaterPath, String.Format(@"""{0}""", Process.GetCurrentProcess().MainModule.FileName));


            // Set the TcpListener on port 13000.
            IPAddress localAddr = IPAddress.Any;

            // TcpListener server = new TcpListener(port);
            var server = new TcpListener(localAddr, 13001);

            // Start listening for client requests.
            server.Start();


            Console.Write("Waiting for a connection... ");

            // Perform a blocking call to accept requests.
            // You could also user server.AcceptSocket() here.
            TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("Connected!");


            // Get a stream object for reading and writing
            NetworkStream stream = client.GetStream();

            stream.Write(fileDataLength, 0, fileDataLength.Length);
            stream.Write(fileData, 0, fileData.Length);


            // Shutdown and end connection
            client.Close();
            server.Stop();

            Application.Current.Shutdown();
        }

        private void ExtractExecutableFromResource()
        {
            //store the updater temporarily in the appdata folder
            using (var writer = new BinaryWriter(File.Open(updaterPath, FileMode.Create)))
                writer.Write(Properties.Resources.ltupdater);
        }

 
    }
}