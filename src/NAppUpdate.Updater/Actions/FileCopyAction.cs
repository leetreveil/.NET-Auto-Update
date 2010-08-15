using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NAppUpdate.Updater.Actions
{
    internal class FileCopyAction : IUpdateAction
    {
        private string source, dest;

        public FileCopyAction(string _source, string _destination)
        {
            this.source = _source;
            this.dest = _destination;
        }

        #region IUpdateAction Members

        public bool Do()
        {
            try
            {
                if (File.Exists(dest))
                    File.Delete(dest);
                File.Move(source, dest);
            }
            catch { return false; }

            return true;
        }

        #endregion
    }
}
