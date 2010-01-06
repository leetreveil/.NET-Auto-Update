using System;
using System.IO;
using leetreveil.AutoUpdate.Core.Zip;
using NUnit.Framework;


namespace leetreveil.AutoUpdate.Tests.Integration.Zip
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


        [Test]
        [ExpectedException(typeof(NonValidZipFileException))]
        public void Should_throw_an_exception_if_file_is_not_a_zip_file()
        {
            new ZipFileExtractor("Samples/adobereference.xml").ExtractTo("Samples/EmptyFolder");
        }

        [Test]
        [ExpectedException(typeof(EmptyZipFileException))]
        public void Should_throw_an_exception_of_the_zip_file_is_empty()
        {
            new ZipFileExtractor("Samples/zipfilewithnofiles.zip").ExtractTo("Samples/EmptyFolder");
        }
    }
}