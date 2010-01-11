using System;

namespace leetreveil.AutoUpdate.Core.UpdateCheck
{
    public class Update
    {
        public string FileUrl { get; set; }
        public Version Version { get; set; }
        public string Title { get; set; }
    }
}