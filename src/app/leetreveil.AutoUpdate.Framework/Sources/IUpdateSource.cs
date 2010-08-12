using System;
using System.Collections.Generic;
using System.Text;

namespace leetreveil.AutoUpdate.Framework.Sources
{
    public interface IUpdateSource
    {
        string GetUpdatesFeed();
        byte[] GetFile(string url);
    }
}
