using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.IO.Pipes;
using System.Windows;
using leetreveil.AutoUpdate.Core.FileDownload;
using leetreveil.AutoUpdate.Core.UpdateCheck;
using Winterdom.IO.FileMap;
using Path=System.IO.Path;
using System.IO;
using System.Text;
using System.Net;
using System.Threading;

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


            Process updateProcess = new Process();
            updateProcess.StartInfo.FileName = updaterPath;
            var thisAppsFilePath = Process.GetCurrentProcess().MainModule.FileName;
            updateProcess.StartInfo.Arguments = String.Format(@"""{0}""", thisAppsFilePath);

            updateProcess.StartInfo.UseShellExecute = true;
            //updateProcess.StartInfo.Verb = "runas";
            updateProcess.Start();


            // Set the TcpListener on port 13000.
            IPAddress localAddr = IPAddress.Any;

            // TcpListener server = new TcpListener(port);
            var server = new TcpListener(localAddr,13001);

            // Start listening for client requests.
            server.Start();


 
                Console.Write("Waiting for a connection... ");

                // Perform a blocking call to accept requests.
                // You could also user server.AcceptSocket() here.
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connected!");


                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();

                var datum = Encoding.ASCII.GetBytes("hello from baltimore!");

                stream.Write(fileDataLength, 0, fileDataLength.Length);
                stream.Write(fileData,0,fileData.Length);


                // Shutdown and end connection
                client.Close();


            server.Stop();
            Application.Current.Shutdown();





            //using (var pipeServer = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable))
            //{


            //    pipeServer.DisposeLocalCopyOfClientHandle();

            //    using (StreamWriter sw = new StreamWriter(pipeServer))
            //    {
            //        sw.AutoFlush = true;
            //        sw.WriteLine("hello from the applicationz!!");
            //    }
            //}

            //var pipeServer = new NamedPipeServerStream("autoUpdatePipe", PipeDirection.Out, 1, PipeTransmissionMode.Byte,
            //                                           PipeOptions.Asynchronous);
   
            //    // Wait for a client to connect
            //    Console.Write("Waiting for client connection...");
            //    pipeServer.BeginWaitForConnection(iar =>
            //                                          {
            //                                              pipeServer.EndWaitForConnection(iar);

            //                                              Console.WriteLine("Client connected.");
            //                                              try
            //                                              {
            //                                                  //send file data to auto update process
            //                                                  using (var sw = new BinaryWriter(pipeServer))
            //                                                      sw.Write(fileData);
            //                                              }
            //                                              // Catch the IOException that is raised if the pipe is 
            //                                              // broken or disconnected.
            //                                              catch (IOException exy)
            //                                              {
            //                                                  Console.WriteLine("ERROR: {0}", exy.Message);
            //                                              }

                                        
            //                                          }, null);





            //map file into memory using mem mapped files

            //MemoryMappedFile map = MemoryMappedFile.Create(MapProtection.PageReadWrite, fileData.Length,
            //                                               @"Local\AutoUpdatePackage2");

            //using (Stream view = map.MapView(MapAccess.FileMapWrite, 0, fileData.Length))
            //    view.Write(fileData, 0, fileData.Length);


            //var file = MemoryMappedFile.Open(MapAccess.FileMapRead, @"Local\AutoUpdatePackage2");
            //var stream = file.MapView(MapAccess.FileMapRead, 0, fileData.Length);



            //StartUpdaterExeAndShutdown(fileData.Length);
        }

        private void ExtractExecutableFromResource()
        {
            //store the updater temporarily in the appdata folder
            using (var writer = new BinaryWriter(File.Open(updaterPath, FileMode.Create)))
                writer.Write(Properties.Resources.ltupdater);
        }

        /// <summary>
        /// Starts the update executable, shuts down the current application so the update can start
        /// </summary>
        private void StartUpdaterExeAndShutdown(int updateFileSize)
        {
            //TODO: fix null reference
            var thisAppsFilePath = Process.GetCurrentProcess().MainModule.FileName;

            //TODO: wrap in try catch because if the user does not accept elevation it will crash
            Process.Start(updaterPath, String.Format(@"""{0}"" {1}", thisAppsFilePath,updateFileSize));

           // Application.Current.Shutdown();
        }
    }
}