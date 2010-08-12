using System;
using System.Net;

namespace leetreveil.AutoUpdate.Framework.Utils
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

        public void DownloadAsync(Action<byte[]> finishedCallback)
        {
            DownloadAsync(finishedCallback, null);
        }

        public void DownloadAsync(Action<byte[]> finishedCallback, Action<long, long> progressChangedCallback)
        {
            using (var client = new WebClient())
            {
                if (progressChangedCallback != null)
                    client.DownloadProgressChanged += (sender, args) => progressChangedCallback(args.BytesReceived, args.TotalBytesToReceive);

                client.DownloadDataCompleted += (sender, args) => finishedCallback(args.Result);
                client.DownloadDataAsync(_uri);
            }
        }
    }
}