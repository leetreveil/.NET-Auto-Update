using System.Collections.Generic;

namespace leetreveil.AutoUpdate.Framework
{
    public interface IUpdateFeedSource
    {
        IEnumerable<Update> Read(string url);
    }
}