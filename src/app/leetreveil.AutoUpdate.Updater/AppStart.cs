using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Windows.Forms;
using leetreveil.AutoUpdate.Updater.Zip;

namespace leetreveil.AutoUpdate.Updater
{
    internal static class AppStart
    {
        private static void Main()
        {
            //Debugger.Launch();

            string[] args = Environment.GetCommandLineArgs();

            var appPath = args[1];

            TcpClient client = new TcpClient("localhost",13001);

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

            ExtractAndStartApplication(appPath,fileData.ToArray());


            Application.Exit();
        }


        private static void ExtractAndStartApplication(string applicationFilePath, byte[] updateData)
        {
            var extractor = new ZipFileExtractor(updateData);
            extractor.ExtractTo(Environment.CurrentDirectory);

            Process.Start(applicationFilePath);
        }
    }



    //using (StreamReader sr = new StreamReader(new AnonymousPipeClientStream(PipeDirection.In,args[2])))
    //{
    //        string line;
    //        while ((line = sr.ReadLine()) != null)
    //        {
    //            Console.WriteLine("Echo: {0}", line);
    //            MessageBox.Show(line);
    //        }
    //}

    //using (var pipeClient = new NamedPipeClientStream(".", "autoUpdatePipe", PipeDirection.In))
    //{

    //    // Connect to the pipe or wait until the pipe is available.
    //    Console.Write("Attempting to connect to pipe...");
    //    pipeClient.Connect();

    //    Console.WriteLine("Connected to pipe.");
    //    Console.WriteLine("There are currently {0} pipe server instances open.",
    //       pipeClient.NumberOfServerInstances);

    //    using (BinaryReader rdr = new BinaryReader(pipeClient))
    //    {
    //        byte[] bytes = new byte[rdr.BaseStream.Length];

    //        var readBytes = rdr.ReadBytes((int)rdr.BaseStream.Length);
    //    }

    //    if (!File.Exists(appPath))
    //        throw new FileNotFoundException("could not find the application file " + appPath);


    //    foreach (var process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension(appPath)))
    //        process.WaitForExit();

    //    ExtractAndStartApplication(appPath, pipeClient);
    //}
}