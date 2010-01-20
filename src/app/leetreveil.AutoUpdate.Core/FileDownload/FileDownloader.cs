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

        public byte[] Download()
        {
            var client = new WebClient();

            return client.DownloadData(_url);
        }
    }
}