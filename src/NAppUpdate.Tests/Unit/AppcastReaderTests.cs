using System;
using System.Linq;
using NAppUpdate.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NAppUpdate.Tests.Unit
{
    [TestClass]
    public class AppcastReaderTests
    {
        [TestMethod]
        public void Should_be_able_to_get_the_title_from_the_update()
        {
            var reader = new AppcastReader();

            Update update = reader.Read(@"Samples\zunesocialtagger.xml").First();

            Assert.Equals(update.Title, "Zune Social Tagger");
        }

        [TestMethod]
        public void Should_be_able_to_get_the_version_no__from_the_update()
        {
            var reader = new AppcastReader();

            Update update = reader.Read(@"Samples\zunesocialtagger.xml").First();

            Assert.Equals(update.Version, new Version(1,2));
        }

        [TestMethod]
        public void Should_be_able_to_get_the_file_url_from_the_update()
        {
            var reader = new AppcastReader();

            Update update = reader.Read(@"Samples\zunesocialtagger.xml").First();

            Assert.Equals(update.FileUrl, "http://cloud.github.com/downloads/leetreveil/Zune-Social-Tagger/Zune_Social_Tagger_1.2.zip");
        }


        [TestMethod]
        public void Should_be_able_to_get_the_updates_file_size_from_the_update()
        {
            var reader = new AppcastReader();

            Update update = reader.Read(@"Samples\zunesocialtagger.xml").First();

            Assert.Equals(update.FileLength, 865843);
        }
    }
}