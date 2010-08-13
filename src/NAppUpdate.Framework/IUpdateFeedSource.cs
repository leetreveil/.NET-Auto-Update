using System.Collections.Generic;

namespace NAppUpdate.Framework
{
    public interface IUpdateFeedSource
    {
        IEnumerable<Update> Read(string url);
    }
}