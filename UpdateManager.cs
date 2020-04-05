
namespace ChaseLabs.CLUpdate
{
    /// <summary>
    /// <para>
    /// Author: Drew Chase
    /// </para>
    /// <para>
    /// Company: Chase Labs
    /// </para>
    /// </summary>
    public class UpdateManager
    {

        /// <summary>
        /// Automatically Checks for Update and Downloads if Needed
        /// <para>Version Files Should be Orginized Like so</para>
        /// <code>Version Key 1.0.0</code>
        /// </summary>
        /// <param name="versionKey">The Label before the Version Numbers</param>
        /// <param name="localVersionFilePath">The Path to the current Version File, If not found it will automatically update</param>
        /// <param name="remoteVersionFilePath">The URL to the updated Version File</param>
        /// <param name="zipUrl">The Direct Download URL to the Update Zip Archive</param>
        /// <param name="zipDirectory">The Temp Directory where the Zip Archive will be Downloaded</param>
        /// <param name="unzipDirectory">The Application Directory</param>
        /// <param name="launchExecutableName">The Application Executable Path</param>
        /// <param name="overwrite">Weather to Clear the Application Directory Prior to Unziping</param>
        public static void Update(string versionKey, string localVersionFilePath, string remoteVersionFilePath, string zipUrl, string zipDirectory, string unzipDirectory, string launchExecutableName, bool overwrite = true)
        {
            if (CheckForUpdate(versionKey, localVersionFilePath, remoteVersionFilePath) || !System.IO.File.Exists(System.IO.Path.Combine(unzipDirectory, launchExecutableName)))
            {
                System.Console.WriteLine("Update Needed!");
                Interfaces.IUpdater update = Updater.Init(zipUrl, zipDirectory, unzipDirectory, launchExecutableName, overwrite);

                update.Download();
                update.Unzip();
                update.CleanUp();

                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    if (System.IO.File.Exists(localVersionFilePath))
                    {
                        System.IO.File.Delete(localVersionFilePath);
                    }

                    client.DownloadFile(remoteVersionFilePath, localVersionFilePath);
                    client.Dispose();
                }
                update.LaunchExecutable();
            }
            else
            {
                System.Console.WriteLine("Your Up to Date!");
                if (new System.Diagnostics.Process() { StartInfo = new System.Diagnostics.ProcessStartInfo() { FileName = System.IO.Path.Combine(unzipDirectory, launchExecutableName) } }.Start())
                {
                    System.Environment.Exit(0);
                }
            }
        }

        /// <summary>
        /// Checks if an Update is Needed
        /// <para>Version Files Should be Orginized Like so</para>
        /// <code>Version Key 1.0.0</code>
        /// </summary>
        /// <param name="versionKey">The Label before the Version Numbers</param>
        /// <param name="localVersionFilePath">The Path to the current Version File, If not found it will automatically update</param>
        /// <param name="remoteVersionFilePath">The URL to the updated Version File</param>
        /// <returns>True = Needs Update | False = Doesn't Need Update</returns>
        public static bool CheckForUpdate(string versionKey, string localVersionFilePath, string remoteVersionFilePath)
        {
            try
            {
                if (!System.IO.File.Exists(localVersionFilePath))
                {
                    return true;
                }

                if (string.IsNullOrEmpty(ReadLocalVersion(localVersionFilePath, versionKey)) || string.IsNullOrWhiteSpace(ReadLocalVersion(localVersionFilePath, versionKey)))
                {
                    return true;
                }

                int currentRelease = ParseVersioning(ReadLocalVersion(localVersionFilePath, versionKey))[0], currentMajor = ParseVersioning(ReadLocalVersion(localVersionFilePath, versionKey))[1], currentMinor = ParseVersioning(ReadLocalVersion(localVersionFilePath, versionKey))[2];
                int remoteRelease = ParseVersioning(ReadRemoteVersion(remoteVersionFilePath, versionKey))[0], remoteMajor = ParseVersioning(ReadRemoteVersion(remoteVersionFilePath, versionKey))[1], remoteMinor = ParseVersioning(ReadRemoteVersion(remoteVersionFilePath, versionKey))[2];

                if (currentMajor == 0 && currentMinor == 0 && currentRelease == 0)
                {
                    return true;
                }

                if (currentRelease < remoteRelease)
                {
                    return true;
                }

                if (currentMajor < remoteMajor)
                {
                    return true;
                }

                if (currentMinor < remoteMinor)
                {
                    return true;
                }
            }
            catch
            {
                return true;
            }

            return false;
        }

        private static int[] ParseVersioning(string version)
        {
            int[] value = { 0, 0, 0 };
            for (int i = 0; i < version.Split('.').Length; i++)
            {
                if (i == 0)
                {
                    int.TryParse(version.Split('.')[i], out value[i]);
                }

                if (i == 1)
                {
                    int.TryParse(version.Split('.')[i], out value[i]);
                }

                if (i == 2)
                {
                    int.TryParse(version.Split('.')[i], out value[i]);
                }
            }
            return value;
        }

        private static string ReadRemoteVersion(string url, string key)
        {
            string value = string.Empty;
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                value = client.DownloadString(url);
                foreach (string s in value.Split(System.Environment.NewLine.ToCharArray()[0]))
                {
                    if (s.StartsWith(key))
                    {
                        value = s.Replace(key, "");
                    }
                }
                client.Dispose();
            }
            return value;
        }

        private static string ReadLocalVersion(string path, string key)
        {
            string value = string.Empty;
            using (System.IO.StreamReader reader = new System.IO.StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    string txt = reader.ReadLine();
                    if (txt.StartsWith(key))
                    {
                        value = txt.Replace(key, "");
                    }
                }
            }
            return value;
        }

    }
}
