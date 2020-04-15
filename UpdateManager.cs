
using ChaseLabs.CLUpdate.Lists;

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

        Versions localVerions;
        Versions remoteVersions;

        /// <summary>
        /// Gets the Local Version Object
        /// </summary>
        public Versions LocalVerions => localVerions;
        /// <summary>
        /// Gets the Remote Version Object
        /// </summary>
        public Versions RemoteVersions => remoteVersions;

        static UpdateManager _singleton;
        /// <summary>
        /// A Static Single Object
        /// </summary>
        public static UpdateManager Singleton
        {
            get
            {
                if (_singleton == null) _singleton = new UpdateManager();
                return _singleton;
            }
        }

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
        public void Update(string versionKey, string localVersionFilePath, string remoteVersionFilePath, string zipUrl, string zipDirectory, string unzipDirectory, string launchExecutableName, bool overwrite = true)
        {
            if (CheckForUpdate(versionKey, localVersionFilePath, remoteVersionFilePath) || !System.IO.File.Exists(System.IO.Path.Combine(unzipDirectory, launchExecutableName)))
            {
                System.Console.WriteLine("Update Needed!");
                Interfaces.IUpdater update = Updater.Init(zipUrl, zipDirectory, unzipDirectory, launchExecutableName, overwrite);

                update.Download();
                update.Unzip();
                update.CleanUp();

                UpdateVersionFile(versionKey);

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
        /// Updates the Local Version to the Updated One.
        /// </summary>
        /// <param name="key"></param>
        public void UpdateVersionFile(string key)
        {
            localVerions.UpdateVersion(key, RemoteVersions.GetVersion(key).Value);
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
        public bool CheckForUpdate(string versionKey, string localVersionFilePath, string remoteVersionFilePath)
        {
            if (System.IO.File.Exists(System.IO.Path.Combine(System.IO.Directory.GetParent(localVersionFilePath).FullName, "Remote Versions"))) System.IO.File.Delete(System.IO.Path.Combine(System.IO.Directory.GetParent(localVersionFilePath).FullName, "Remote Versions"));
            localVerions = new Versions(localVersionFilePath);
            remoteVersions = new Versions(System.IO.Path.Combine(System.IO.Directory.GetParent(localVersionFilePath).FullName, "Remote Versions"));
            try
            {
                if (localVerions.GetVersion(versionKey) == null || localVerions.GetVersion(versionKey).Value == "")
                {
                    localVerions.AddVersion(new Objects.Version() { Key = versionKey, Value = "0.0.0" });
                    return true;
                }

                if (!System.IO.File.Exists(localVersionFilePath))
                {
                    localVerions.AddVersion(new Objects.Version() { Key = versionKey, Value = "0.0.0" });
                    return true;
                }

                if (string.IsNullOrEmpty(ReadLocalVersion(versionKey)) || string.IsNullOrWhiteSpace(ReadLocalVersion(versionKey)))
                {
                    localVerions.AddVersion(new Objects.Version() { Key = versionKey, Value = "0.0.0" });
                    return true;
                }

                int currentRelease = ParseVersioning(ReadLocalVersion(versionKey))[0], currentMajor = ParseVersioning(ReadLocalVersion(versionKey))[1], currentMinor = ParseVersioning(ReadLocalVersion(versionKey))[2];
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

        private int[] ParseVersioning(string version)
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

        private string ReadRemoteVersion(string url, string key)
        {
            string value = "";
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                client.DownloadFile(url, remoteVersions.Path);
                remoteVersions.VersionManager.FindPreExistingConfigs();

                if (remoteVersions.GetVersion(key) != null)
                {
                    value = remoteVersions.GetVersion(key).Value;
                }
                else
                {
                    remoteVersions.AddVersion(new Objects.Version() { Key = key, Value = "0.0.0" });
                }

                client.Dispose();
            }
            if (System.IO.File.Exists(remoteVersions.Path))
                System.IO.File.Delete(remoteVersions.Path);
            return value;
        }

        private string ReadLocalVersion(string key)
        {
            if (localVerions.GetVersion(key) == null)
            {
                localVerions.AddVersion(new Objects.Version() { Key = key, Value = "0.0.0" });
            }
            return localVerions.GetVersion(key).Value;
        }

    }
}
