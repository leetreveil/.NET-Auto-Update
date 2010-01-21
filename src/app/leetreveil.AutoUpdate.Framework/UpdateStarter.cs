using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace leetreveil.AutoUpdate.Framework
{
    public class UpdateStarter
    {
        private readonly string _updaterExePath;
        private readonly byte[] _updateExe;

        public UpdateStarter(string updatePath, byte[] updateExe)
        {
            _updaterExePath = updatePath;
            _updateExe = updateExe;
        }

        public void Start(string updatePackageUrl)
        {
            var fDownloader = new FileDownloader(updatePackageUrl);

            byte[] fileData = fDownloader.Download();

            ExtractExecutableFromResource();

            //TODO: if the user does not accept elevation prompt we get an error
            Process.Start(_updaterExePath, String.Format(@"""{0}""", Process.GetCurrentProcess().MainModule.FileName));

            SendUpdatePackageToUpdateExecutable(fileData);
        }

        private static void SendUpdatePackageToUpdateExecutable(byte[] fileData)
        {
            var server = new TcpListener(IPAddress.Any, 13001);
            server.Start();

            //wait for the updater to start and attempt to connect
            TcpClient client = server.AcceptTcpClient();

            using (NetworkStream stream = client.GetStream())
            {
                var fileDataLength = BitConverter.GetBytes(fileData.Length);
                //send the size of the update package as the first 4 bytes
                stream.Write(fileDataLength, 0, 4);
                //send the rest of the file
                stream.Write(fileData, 0, fileData.Length);
            }

            // Shutdown connection and stop listening
            client.Close();
            server.Stop();
        }

        private void ExtractExecutableFromResource()
        {
            //store the updater temporarily in the appdata folder
            using (var writer = new BinaryWriter(File.Open(_updaterExePath, FileMode.Create)))
                writer.Write(_updateExe);
        }
    }
}