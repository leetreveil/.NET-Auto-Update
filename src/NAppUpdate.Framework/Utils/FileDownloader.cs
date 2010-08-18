using System;
using System.Net;

namespace NAppUpdate.Framework.Utils
{
    public sealed class FileDownloader
    {
        private readonly Uri _uri;

        public FileDownloader(string url)
        {
            _uri = new Uri(url);
        }

        public FileDownloader(Uri uri)
        {
            _uri = uri;
        }

        public byte[] Download()
        {
            using (var client = new WebClient())
                return client.DownloadData(_uri);
        }

        public bool DownloadToFile(string tempLocation)
        {
            using (var client = new WebClient())
                client.DownloadFile(_uri, tempLocation);

            return true;
        }

        /*
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
        }*/
    }
}