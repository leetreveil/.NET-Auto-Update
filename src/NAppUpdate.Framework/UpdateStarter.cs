using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace NAppUpdate.Framework
{
    /// <summary>
    /// Downloads and starts the update process
    /// </summary>
    public class UpdateStarter
    {
        private readonly string _updaterExeExeShouldBeCreated;
        private readonly byte[] _updateExe;
        private readonly byte[] _updateData;

        public UpdateStarter(string pathWhereUpdateExeShouldBeCreated, byte[] updateExe, byte[] updateData)
        {
            _updaterExeExeShouldBeCreated = pathWhereUpdateExeShouldBeCreated;
            _updateExe = updateExe;
            _updateData = updateData;
        }

        public void Start()
        {
            ExtractExecutableFromResource(); //take the update executable and extract it to the path where it should be created

            try
            {
                //TODO: allow custom port or randomise instead of using a default
                Process.Start(_updaterExeExeShouldBeCreated, String.Format(@"""{0}""", Process.GetCurrentProcess().MainModule.FileName));
                SendUpdatePackageToUpdateExecutable(_updateData);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
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
            using (var writer = new BinaryWriter(File.Open(_updaterExeExeShouldBeCreated, FileMode.Create)))
                writer.Write(_updateExe);
        }
    }
}