using System.IO;
using System.Security.Principal;
using System.Security.AccessControl;

namespace NAppUpdate.Framework.Utils
{
    public static class PermissionsCheck
    {
        private static readonly IdentityReferenceCollection groups = WindowsIdentity.GetCurrent().Groups;
        private static readonly string sidCurrentUser = WindowsIdentity.GetCurrent().User.Value;

        public static bool HaveWritePermissionsForFolder(string path)
        {
            var rules = Directory.GetAccessControl(Path.GetDirectoryName(Path.GetDirectoryName(path))).GetAccessRules(true, true, typeof(SecurityIdentifier));

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

            // If we have both allow and deny permissions, the deny takes precident.
            if (allowwrite && !denywrite)
                return true;

            return false;
        }
    }
}
