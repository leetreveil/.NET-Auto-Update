using System;
using System.Net;

namespace leetreveil.AutoUpdate.Framework
{
    public class FileDownloader
    {
        private readonly Uri _uri;

        public FileDownloader(string url)
        {
            _uri = new Uri(url);
        }

        public byte[] Download()
        {
            using (var client = new WebClient())
                return client.DownloadData(_uri);
        }

        public void DownloadAsync(Action<byte[]> callback)
        {
            using (var client = new WebClient())
            {
                client.DownloadDataCompleted += (sender, args) => callback(args.Result);
                client.DownloadDataAsync(_uri);
            }
        }

        public void DownloadAsync(Action<byte[]> finishedCallback, Action<Progress> progressChangedCallback)
        {
            using (var client = new WebClient())
            {
                client.DownloadProgressChanged += (sender, args) => 
                    progressChangedCallback(new Progress { Current = args.BytesReceived, Total = args.TotalBytesToReceive });

                client.DownloadDataCompleted += (sender, args) => finishedCallback(args.Result);
                client.DownloadDataAsync(_uri);
            }
        }
    }
}