using System;
using System.Net;

namespace leetreveil.AutoUpdate.Framework
{
    public class FileDownloader
    {
        private readonly string _url;

        public FileDownloader(string url)
        {
            _url = url;
        }

        public byte[] Download()
        {
            var client = new WebClient();

            var downloadedData = client.DownloadData(new Uri(_url));

            return downloadedData;
        }
    }
}