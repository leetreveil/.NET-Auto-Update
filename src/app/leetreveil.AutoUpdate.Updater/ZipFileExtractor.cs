using System.IO;
using Ionic.Zip;

namespace leetreveil.AutoUpdate.Updater
{
    public class ZipFileExtractor
    {
        private readonly byte[] _updateData;

        public ZipFileExtractor(byte[] updateData)
        {
            _updateData = updateData;
        }

        public void ExtractTo(string folderPath)
        {
            using (ZipFile extractedFiles = ZipFile.Read(_updateData))
            {
                foreach (var file in extractedFiles)
                    file.Extract(folderPath, ExtractExistingFileAction.OverwriteSilently);
            }
        }
    }
}