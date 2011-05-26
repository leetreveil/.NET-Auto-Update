using System;
using System.IO;
using System.Net;

namespace NAppUpdate.Framework.Utils
{
    public sealed class FileDownloader
    {
        private readonly Uri _uri;

		//public event Action<UpdateStatus> StateChanged = delegate { };

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
        	var request = WebRequest.Create(_uri);

			using (var response = request.GetResponse())
			using (var tempFile = File.Create(tempLocation))
			{
				if (response == null)
					return false;

				using (var responseStream = response.GetResponseStream())
				{
					if (responseStream ==null)
						return false;

					var downloadSize = Convert.ToDouble(response.ContentLength);
					var totalBytes = 0;
					var buffer = new byte[1024];
					int bytesRead;
					do
					{
						bytesRead = responseStream.Read(buffer, 0, buffer.Length);
						totalBytes += bytesRead;
						tempFile.Write(buffer, 0, bytesRead);
					} while (bytesRead > 0 && !UpdateManager.Instance.ShouldStop);

					return totalBytes == downloadSize;
				}
			}
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