using Ionic.Zip;

namespace leetreveil.AutoUpdate.Updater.Zip
{
    public class ZipFileExtractor
    {
        private readonly string _filePath;

        public ZipFileExtractor(string filePath)
        {
            _filePath = filePath;

            if (!ZipFile.IsZipFile(_filePath))
                throw new NonValidZipFileException("not a valid zip file");
        }

        public void ExtractTo(string folderPath)
        {
            ZipFile extractedFiles = ZipFile.Read(_filePath);

            if (extractedFiles.Count == 0)
                throw new EmptyZipFileException("zip file has no files");

            foreach (var file in extractedFiles)
                file.Extract(folderPath,ExtractExistingFileAction.OverwriteSilently);
        }
    }
}