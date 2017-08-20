using System;
using System.IO;

namespace GitlabCmd.Console.Utilities
{
    public static class EnvironmentExtensions
    {
        public static string GetLocalAppDataFolder(this string appName)
        {
            string localAppDataFolder = Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData);

            return Path.Combine(localAppDataFolder, appName);
        }
    }
}
