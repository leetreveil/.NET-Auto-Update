using System;
using System.IO;
using leetreveil.AutoUpdate.Updater.Zip;
using NUnit.Framework;

namespace leetreveil.AutoUpdate.Tests.Integration.Updater.Zip
{
    [TestFixture]
    public class ZipFileExtractorTests
    {
        [Test]
        public void Should_be_able_to_extract_the_contents_of_a_zip_file_to_an_empty_folder()
        {
            var extractor = new ZipFileExtractor("Samples/zipfilewithonefile.zip");

            string emptyfolderPath = "Samples/EmptyFolder";

            if (Directory.Exists(emptyfolderPath))
                Directory.Delete(emptyfolderPath, true);

            extractor.ExtractTo(emptyfolderPath);

            Assert.That(File.Exists(Path.Combine(emptyfolderPath,"zunesocialtagger.xml")));
        }

        [Test]
        public void Should_be_able_to_extract_the_contents_of_a_zip_file_and_overwrite_any_files_with_the_same_filename_in_the_destination_folder()
        {
            var extractor = new ZipFileExtractor("Samples/zipfilewithonefile.zip");

            string emptyfolderPath = "Samples/EmptyFolder";

            if (Directory.Exists(emptyfolderPath))
                Directory.Delete(emptyfolderPath, true);

            //we are extracting twice to simulate the replacing of files
            //setup folder structure
            extractor.ExtractTo(emptyfolderPath);
            //replace
            extractor.ExtractTo(emptyfolderPath);

            Assert.That(File.Exists(Path.Combine(emptyfolderPath, "zunesocialtagger.xml")));
        }

        [Test]
        public void Should_be_able_to_update_subfolders_contents()
        {
            throw new NotImplementedException();
        }
    }
}