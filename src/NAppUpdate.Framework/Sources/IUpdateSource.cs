using System;
using System.Collections.Generic;
using System.Text;

namespace NAppUpdate.Framework.Sources
{
    public interface IUpdateSource
    {
        string GetUpdatesFeed();
        byte[] GetFile(string filePath, string basePath);
    }
}
