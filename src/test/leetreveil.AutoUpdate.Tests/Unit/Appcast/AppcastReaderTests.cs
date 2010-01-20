using System.Linq;
using leetreveil.AutoUpdate.Core;
using leetreveil.AutoUpdate.Core.Appcast;
using NUnit.Framework;

namespace leetreveil.AutoUpdate.Tests.Unit.Appcast
{
    [TestFixture]
    public class AppcastReaderTests
    {
        [Test]
        public void Should_be_able_to_get_the_newest_items_title_from_the_appcast_feed()
        {
            var reader = new AppcastReader();

            Update update = reader.Read(@"Samples\zunesocialtagger.xml").First();

            Assert.That(update.Title, Is.EqualTo("Zune Social Tagger"));
        }

        [Test]
        public void Should_be_able_to_get_the_newest_items_version_no_from_the_appcast_feed()
        {
            var reader = new AppcastReader();

            Update update = reader.Read(@"Samples\zunesocialtagger.xml").First();

            Assert.That(update.Version, Is.EqualTo("1.2"));
        }

        [Test]
        public void Should_be_able_to_get_the_newest_items_file_url_from_the_appcast_feed()
        {
            var reader = new AppcastReader();

            Update update = reader.Read(@"Samples\zunesocialtagger.xml").First();

            Assert.That(update.FileUrl, Is.EqualTo("http://cloud.github.com/downloads/leetreveil/Zune-Social-Tagger/Zune_Social_Tagger_1.2.zip"));
        }
    }
}
