using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NAppUpdate.Updater.Actions
{
    public class FileDumpAction : IUpdateAction
    {
        private string filePath;
        private byte[] fileData;

        public FileDumpAction(string path, byte[] data)
        {
            this.filePath = path;
            this.fileData = data;
        }

        #region IUpdateAction Members

        public bool Do()
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(fileData, 0, fileData.Length);
                }
            }
            catch { return false; }
            return true;
        }

        #endregion
    }
}
