using System;
using System.Collections.Generic;
using System.Text;

namespace NAppUpdate.Framework.Sources
{
    public interface IUpdateSource
    {
        string GetUpdatesFeed();
        bool GetData(string filePath, string basePath, ref string tempLocation);
    }
}
