using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Windows.Forms;

namespace leetreveil.AutoUpdate.Updater
{
    internal static class AppStart
    {
        private static void Main()
        {
            //Debugger.Launch
            try
            {
                string[] args = Environment.GetCommandLineArgs();

                var appPath = args[1];

                var client = new TcpClient("localhost", 13001);

                var stream = client.GetStream();

                byte[] fileDataLength = new byte[4];
                stream.Read(fileDataLength, 0, 4);
                int fileLength = BitConverter.ToInt32(fileDataLength, 0);


                var fileData = new List<byte>();

                while (fileData.Count < fileLength)
                {
                    byte[] someMessage = new byte[client.Available];
                    stream.Read(someMessage, 0, someMessage.Length);
                    fileData.AddRange(someMessage);
                }

                stream.Close();
                client.Close();


                foreach (var process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension(appPath)))
                    process.WaitForExit();

                //overwrite the application files
                ApplyUpdate(fileData.ToArray());

                //start the application
                Process.Start(appPath);
            }
            catch
            {
                //supressing catch because if at any point we get an error the update has failed
            }
            finally
            {
                Application.Exit();
            }
        }


        private static void ApplyUpdate(byte[] updateData)
        {
            var extractor = new ZipFileExtractor(updateData);
            extractor.ExtractTo(Environment.CurrentDirectory);
        }
    }
}