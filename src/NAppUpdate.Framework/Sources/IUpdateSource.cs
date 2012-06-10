using System;
using NAppUpdate.Framework.Common;

namespace NAppUpdate.Framework.Sources
{


    public interface IUpdateSource
    {
        string GetUpdatesFeed();
		bool GetData(string filePath, string basePath, Action<UpdateProgressInfo> onProgress, ref string tempLocation);
    }
}
