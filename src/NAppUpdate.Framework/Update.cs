using System;

namespace NAppUpdate.Framework
{
    public class Update
    {
        public string FileUrl { get; set; }
        public Version Version { get; set; }
        public string Title { get; set; }
        public long FileLength { get; set; }
    }
}