using System.Collections.Generic;

namespace leetreveil.AutoUpdate.Framework
{
    public interface IUpdateFeedSource
    {
        List<Update> Read(string url);
    }
}