using System.Collections.Generic;

namespace leetreveil.AutoUpdate.Core.Appcast
{
    public interface IUpdateFeedSource
    {
        IEnumerable<Update> Read(string url);
    }
}