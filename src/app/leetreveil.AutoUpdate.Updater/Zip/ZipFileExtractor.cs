using Ionic.Zip;

namespace leetreveil.AutoUpdate.Updater.Zip
{
    public class ZipFileExtractor
    {
        private readonly string _filePath;

        public ZipFileExtractor(string filePath)
        {
            _filePath = filePath;
        }

        public void ExtractTo(string folderPath)
        {
            if (!ZipFile.IsZipFile(_filePath))
                return;

            using (ZipFile extractedFiles = ZipFile.Read(_filePath))
            {
                foreach (var file in extractedFiles)
                    file.Extract(folderPath, ExtractExistingFileAction.OverwriteSilently);
            }
        }
    }
}