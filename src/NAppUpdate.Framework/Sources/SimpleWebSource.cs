using System;
using System.Collections.Generic;
using System.Text;

using NAppUpdate.Framework.Utils;
using System.Net;

namespace NAppUpdate.Framework.Sources
{
    public class SimpleWebSource : IUpdateSource
    {
        public SimpleWebSource() { }
        public SimpleWebSource(string feedUrl)
        {
            this.FeedUrl = feedUrl;
        }

        public string FeedUrl { get; set; }

        private readonly string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());

        #region IUpdateSource Members

        public string GetUpdatesFeed()
        {
            string data;
            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                data = client.DownloadString(FeedUrl);
            }

            if (data.StartsWith(_byteOrderMarkUtf8))
                data = data.Remove(0, _byteOrderMarkUtf8.Length);

            return data;
        }

        public byte[] GetFile(string url, string baseUrl)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                return new FileDownloader(url).Download();
            else if (!string.IsNullOrEmpty(baseUrl))
                return new FileDownloader(new Uri(new Uri(baseUrl, UriKind.Absolute), url)).Download();
            return null;
        }

        #endregion
    }
}
