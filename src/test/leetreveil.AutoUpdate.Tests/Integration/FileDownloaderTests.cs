using leetreveil.AutoUpdate.Framework;
using NUnit.Framework;
using leetreveil.AutoUpdate.Framework.Utils;

namespace leetreveil.AutoUpdate.Tests.Integration
{
    [TestFixture]
    public class FileDownloaderTests
    {
        [Test]
        public void Should_be_able_to_download_a_small_file_from_the_internet()
        {
            var fileDownloader = new FileDownloader("http://www.google.co.uk/intl/en_uk/images/logo.gif");

            byte[] fileData = fileDownloader.Download();

            Assert.That(fileData.Length,Is.GreaterThan(0));
        }

    }
}