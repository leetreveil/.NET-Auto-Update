using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using leetreveil.AutoUpdate.Core.Appcast;
using NUnit.Framework;

namespace leetreveil.AutoUpdate.Tests.Unit.Appcast
{
    [TestFixture]
    public class AppcastReaderTests
    {
        [Test]
        public void Should_be_able_to_get_the_newest_item_from_the_appcast_feed()
        {
            var reader = new AppcastReader(new Uri(@"Samples/zunesocialtagger.xml"));

            AppcastItem update = reader.Read().First();

            Assert.That(update.Title, Is.EqualTo("Zune Social Tagger"));
            Assert.That(update.Version,Is.EqualTo("1.2"));
        }
    }
}
