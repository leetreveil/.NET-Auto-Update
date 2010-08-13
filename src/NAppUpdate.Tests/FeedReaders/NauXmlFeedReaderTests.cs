using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NAppUpdate.Framework;

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
<feed>
  <title>My application</title>
  <link>http://myapp.com/</link>
  <tasks>
    <task type=""fileUpdate"">
      <description>update details</description>
      <condition check=""exists"" localPath=""c:\test.txt"" />
    </task>
  </tasks>
</feed>";

        [TestMethod]
        public void TestNauReaderCanReadFeed()
        {
            var reader = new NAppUpdate.Framework.FeedReaders.NauXmlFeedReader();
            var updates = reader.Read(UpdateManager.Instance, NauUpdateFeed);

            Assert.IsTrue(updates.Count() > 0);
        }
    }
}
