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

        [TestMethod]
        public void TestNauReaderCanReadFeed()
        {
            var reader = new NAppUpdate.Framework.FeedReaders.NauXmlFeedReader();
            IList<IUpdateTask> updates = reader.Read(NauUpdateFeed);

            Assert.IsTrue(updates.Count > 0);
        }
    }
}
