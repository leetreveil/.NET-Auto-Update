using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Mime;
using System.Text;
using System.Windows.Forms;
using leetreveil.AutoUpdate.Updater.Zip;
using Winterdom.IO.FileMap;

namespace leetreveil.AutoUpdate.Updater
{
    static class AppStart
    {

        static void Main()
        {
            var mappedFile = MemoryMappedFile.Open(MapAccess.FileMapRead, @"Local\MyMappedFile");

            var stream = mappedFile.MapView(MapAccess.FileMapRead, 0, 6);

            byte[] messageBytes = new byte[6];

            stream.Read(messageBytes, 0, 6);

            MessageBox.Show("From the updater! " + Encoding.ASCII.GetString(messageBytes));

            stream.Close();

            Debugger.Launch();

            string[] args = Environment.GetCommandLineArgs();

            var appPath = args[1];

            //if (!File.Exists(compressedUpdateFile))
            //    throw new FileNotFoundException("could not find the update file, did it download correctly? " +
            //                                    compressedUpdateFile);

            if (!File.Exists(appPath))
                throw new FileNotFoundException("could not find the application file " + appPath);


            //TODO: wrap in try catch because of potential violations
            foreach (var process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension(appPath)))
                process.WaitForExit();

           ExtractAndStartApplication(appPath);

            ////TODO: wrap in try catch as it could fail if we do not have access rights
            //if (File.Exists(compressedUpdateFile))
            //    File.Delete(compressedUpdateFile);

            Application.Exit();
        }



        private static void ExtractAndStartApplication(string applicationFilePath)
        {
            //var extractor = new ZipFileExtractor(updateFilePath);
            //extractor.ExtractTo(Environment.CurrentDirectory);

            Process.Start(applicationFilePath);
        }
    }
    
}
