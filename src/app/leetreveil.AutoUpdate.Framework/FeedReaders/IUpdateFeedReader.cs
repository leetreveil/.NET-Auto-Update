using System;
using System.Collections.Generic;
using System.Text;
using leetreveil.AutoUpdate.Framework.Tasks;

namespace leetreveil.AutoUpdate.Framework.FeedReaders
{
    public interface IUpdateFeedReader
    {
        IEnumerable<IUpdateTask> Read(UpdateManager caller, string feed);
    }
}
