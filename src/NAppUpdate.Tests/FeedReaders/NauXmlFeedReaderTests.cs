using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NAppUpdate.Framework;
using NAppUpdate.Framework.Tasks;

namespace NAppUpdate.Tests.FeedReaders
{
    /// <summary>
    /// Summary description for NauXmlFeedReaderTest
    /// </summary>
    [TestClass]
    public class NauXmlFeedReaderTests
    {
        [TestMethod]
        public void NauReaderCanReadFeed1()
        {
        	const string NauUpdateFeed =
        		@"<?xml version=""1.0"" encoding=""utf-8""?>
<Feed>
  <Title>My application</Title>
  <Link>http://myapp.com/</Link>
  <Tasks>
    <FileUpdateTask localPath=""test.dll"" updateTo=""remoteFile.dll"">
      <Description>update details</Description>
      <Conditions>
        <FileExistsCondition localPath=""otherFile.dll"" />
      </Conditions>
    </FileUpdateTask>
  </Tasks>
</Feed>";

            var reader = new NAppUpdate.Framework.FeedReaders.NauXmlFeedReader();
            IList<IUpdateTask> updates = reader.Read(NauUpdateFeed);

            Assert.IsTrue(updates.Count == 1);

        	var task = updates[0] as FileUpdateTask;
			Assert.IsNotNull(task);

			Assert.AreEqual(1, task.UpdateConditions.ChildConditionsCount);

			Assert.IsFalse(task.CanHotSwap);
        }

		[TestMethod]
		public void NauReaderCanReadFeed2()
		{
			const string NauUpdateFeed =
				@"<?xml version=""1.0"" encoding=""utf-8""?>
<Feed>
  <Title>My application</Title>
  <Link>http://myapp.com/</Link>
  <Tasks>
    <FileUpdateTask localPath=""test.dll"" updateTo=""remoteFile.dll"" hotswap=""true"">
      <Description>update details</Description>
      <Conditions>
        <FileExistsCondition localPath=""otherFile.dll"" />
      </Conditions>
    </FileUpdateTask>
  </Tasks>
</Feed>";

			var reader = new NAppUpdate.Framework.FeedReaders.NauXmlFeedReader();
			IList<IUpdateTask> updates = reader.Read(NauUpdateFeed);

			Assert.IsTrue(updates.Count == 1);

			var task = updates[0] as FileUpdateTask;
			Assert.IsNotNull(task);

			Assert.AreEqual(1, task.UpdateConditions.ChildConditionsCount);

			Assert.IsTrue(task.CanHotSwap);
		}
    }
}
