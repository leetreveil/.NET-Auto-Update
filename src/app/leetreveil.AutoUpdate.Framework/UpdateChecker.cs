using System;

namespace leetreveil.AutoUpdate.Framework
{
    public static class UpdateChecker
    {
        /// <summary>
        /// Checks for update with the default settings for Assembly versioning
        /// </summary>
        /// <param name="versionToCheckAgainst"></param>
        /// <param name="updateVersion"></param>
        /// <returns></returns>
        public static bool CheckForUpdate(Version versionToCheckAgainst, Version updateVersion)
        {
            return updateVersion > versionToCheckAgainst;
        }
    }
}