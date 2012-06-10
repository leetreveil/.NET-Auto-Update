using System;
using System.IO;
using System.Net;
using NAppUpdate.Framework.Common;

namespace NAppUpdate.Framework.Utils
{
    public sealed class FileDownloader
    {
        private readonly Uri _uri;
    	private readonly int _bufferSize = 1024;
    	public IWebProxy Proxy { get; set; }

    	public FileDownloader()
    	{
    		Proxy = null;
    	}


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
			return DownloadToFile(tempLocation, null);
		}

		public bool DownloadToFile(string tempLocation, Action<UpdateProgressInfo> onProgress)
        {
        	var request = WebRequest.Create(_uri);
			request.Proxy = Proxy;

			using (var response = request.GetResponse())
			using (var tempFile = File.Create(tempLocation))
			{
				using (var responseStream = response.GetResponseStream())
				{
					if (responseStream ==null)
						return false;

					var downloadSize = response.ContentLength;
					var totalBytes = 0;
					var buffer = new byte[_bufferSize];
					int bytesRead;
					do
					{
						bytesRead = responseStream.Read(buffer, 0, buffer.Length);
						totalBytes += bytesRead;
						tempFile.Write(buffer, 0, bytesRead);

						if (onProgress != null) onProgress(new DownloadProgressInfo {
							DownloadedInBytes = totalBytes,
							FileSizeInBytes = downloadSize,
							Percentage = (int)(totalBytes * 100 / downloadSize),
							Message = string.Format("Downloading... ({0} / {1} completed)", totalBytes, downloadSize), // TODO: KB / MB Formatting
							StillWorking = totalBytes == downloadSize,
						});

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