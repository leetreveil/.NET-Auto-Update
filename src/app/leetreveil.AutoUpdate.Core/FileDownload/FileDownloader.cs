using System;
using System.Net;
using System.IO;

namespace leetreveil.AutoUpdate.Core.FileDownload
{
    public class FileDownloader
    {
        private readonly string _url;

        public FileDownloader(string url)
        {
            _url = url;
        }

        public string Download()
        {
            try
            {
                var client = new WebClient();

                //gets the filename from the end of the url
                var fileNameToSaveAs = Path.GetFileName(_url);

                client.DownloadFile(_url, fileNameToSaveAs);

                return fileNameToSaveAs;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());

                return String.Empty;
            }
        }
    }
}