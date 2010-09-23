using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

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

            // First we need to check whether we have writable permissions to this folder, as these are separate to delete permissions.
            var rules = Directory.GetAccessControl(Path.GetDirectoryName(dest)).GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
            var groups = WindowsIdentity.GetCurrent().Groups;
            string sidCurrentUser = WindowsIdentity.GetCurrent().User.Value;

            bool allowwrite = false;
            bool denywrite = false;
            foreach (FileSystemAccessRule rule in rules)
            {
                if (rule.AccessControlType == AccessControlType.Deny &&
                    (rule.FileSystemRights & FileSystemRights.WriteData) == FileSystemRights.WriteData &&
                    (groups.Contains(rule.IdentityReference) || rule.IdentityReference.Value == sidCurrentUser)
                    )
                {
                    denywrite = true;
                }
                if (rule.AccessControlType == AccessControlType.Allow &&
                    (rule.FileSystemRights & FileSystemRights.WriteData) == FileSystemRights.WriteData &&
                    (groups.Contains(rule.IdentityReference) || rule.IdentityReference.Value == sidCurrentUser)
                    )
                {
                    allowwrite = true;
                }
            }

            // Only proceed if we can write to the dir
            if (allowwrite && !denywrite)
            {

                if (File.Exists(dest))
                {
                    int retries = 5;
                    bool access = false;

                    while (!access && retries > 0)
                    {
                        try
                        {
                            File.Delete(dest);
                            access = true;
                        }
                        catch (System.UnauthorizedAccessException)
                        {
                            // Since we are root this should only happen if the file is locked, so lets sleep & retry
                            retries--;
                            System.Threading.Thread.Sleep(1000);
                        }
                    }

                    if (!access)
                        throw new Exception("Couldn't gain access to " + dest + " to delete it");
                }
                File.Move(source, dest);
            }
            else
            {
                throw new UnauthorizedAccessException("No write permission available on the file");
            }
            return true;
        }
        #endregion
    }
}

