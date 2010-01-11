using System;
using System.Net;

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

                var fileNameToSaveAs = ExtractFileNameFromString(_url);

                client.DownloadFile(_url, fileNameToSaveAs);

                return fileNameToSaveAs;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());

                return String.Empty;
            }
        }

        private string ExtractFileNameFromString(string url)
        {
            return url.Substring(url.LastIndexOf('/') + 1);
        }
    }
}