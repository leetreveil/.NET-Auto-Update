using System;
using System.Linq;
using NAppUpdate.Framework;
using NUnit.Framework;

namespace NAppUpdate.Tests.Unit
{
    [TestFixture]
    public class AppcastReaderTests
    {
        [Test]
        public void Should_be_able_to_get_the_title_from_the_update()
        {
            var reader = new AppcastReader();

            Update update = reader.Read(@"Samples\zunesocialtagger.xml").First();

            Assert.That(update.Title, Is.EqualTo("Zune Social Tagger"));
        }

        [Test]
        public void Should_be_able_to_get_the_version_no__from_the_update()
        {
            var reader = new AppcastReader();

            Update update = reader.Read(@"Samples\zunesocialtagger.xml").First();

            Assert.That(update.Version, Is.EqualTo(new Version(1,2)));
        }

        [Test]
        public void Should_be_able_to_get_the_file_url_from_the_update()
        {
            var reader = new AppcastReader();

            Update update = reader.Read(@"Samples\zunesocialtagger.xml").First();

            Assert.That(update.FileUrl, Is.EqualTo("http://cloud.github.com/downloads/leetreveil/Zune-Social-Tagger/Zune_Social_Tagger_1.2.zip"));
        }


        [Test]
        public void Should_be_able_to_get_the_updates_file_size_from_the_update()
        {
            var reader = new AppcastReader();

            Update update = reader.Read(@"Samples\zunesocialtagger.xml").First();

            Assert.That(update.FileLength, Is.EqualTo(865843));
        }
    }
}