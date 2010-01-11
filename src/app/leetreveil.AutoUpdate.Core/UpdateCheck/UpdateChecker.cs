using System;
using System.Reflection;
using System.Linq;
using leetreveil.AutoUpdate.Core.Appcast;

namespace leetreveil.AutoUpdate.Core.UpdateCheck
{
    public class UpdateChecker
    {
        public Update Update { get; private set; }

        /// <summary>
        /// Checks for update with the default settings for Assembly versioning, minor version resolution
        /// </summary>
        /// <param name="updateUrl"></param>
        /// <returns></returns>
        public bool CheckForUpdate(string updateUrl)
        {
            var appVersion = Assembly.GetEntryAssembly().GetName().Version;

            try
            {
                var appcastReader = new AppcastReader(updateUrl);
                var updates = appcastReader.Read();

                AppcastItem firstUpdate = updates.First();
                var firstUpdateVers = new Version(firstUpdate.Version);

                //so 1.2 > 1.1 && 1.2.1 > 1.1.1
                if (firstUpdateVers.Major >= appVersion.Major && firstUpdateVers.Minor > appVersion.Minor)
                {
                    //update is available
                    this.Update = new Update{FileUrl = firstUpdate.FileUrl,Title = firstUpdate.Title,Version = firstUpdateVers};

                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return true;
        }
    }
}