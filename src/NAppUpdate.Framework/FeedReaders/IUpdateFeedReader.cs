using System;
using System.Collections.Generic;
using System.Text;
using NAppUpdate.Framework.Tasks;

namespace NAppUpdate.Framework.FeedReaders
{
    public interface IUpdateFeedReader
    {
        IList<IUpdateTask> Read(string feed);
    }
}
