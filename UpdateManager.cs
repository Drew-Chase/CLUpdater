using ChaseLabs.CLUpdate.Lists;
using System;
using System.IO;

namespace ChaseLabs.CLUpdate
{
    /// <summary>
    /// <para> Author: Drew Chase </para>
    /// <para> Company: Chase Labs </para>
    /// </summary>
    public class UpdateManager
    {
        #region Private Fields

        private static UpdateManager _singleton;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// A Static Single Object
        /// </summary>
        public static UpdateManager Singleton
        {
            get
            {
                if (_singleton == null)
                {
                    _singleton = new UpdateManager();
                }

                return _singleton;
            }
        }

        /// <summary>
        /// Gets the Local Version Object
        /// </summary>
        public Versions LocalVerions { get; private set; }

        /// <summary>
        /// Gets the Remote Version Object
        /// </summary>
        public Versions RemoteVersions { get; private set; }

        #endregion Public Properties

        #region Private Properties

        private string LocalVersionPath { get; set; }

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Checks if an Update is Needed
        /// <para> Version Files Should be Orginized Like so </para>
        /// <code>Version Key 1.0.0 </code>
        /// </summary>
        /// <param name="versionKey">            The Label before the Version Numbers </param>
        /// <param name="localVersionFilePath"> 
        /// The Path to the current Version File, If not found it will automatically update
        /// </param>
        /// <param name="remoteVersionFilePath"> The URL to the updated Version File </param>
        /// <returns> True = Needs Update | False = Doesn't Need Update </returns>
        public bool CheckForUpdate(string versionKey, string localVersionFilePath, string remoteVersionFilePath)
        {
            if (LocalVersionPath == "")
            {
                Init(remoteVersionFilePath, localVersionFilePath);
            }

            bool needsUpdate = false;
            try
            {
                if (LocalVerions.GetVersion(versionKey) == null || LocalVerions.GetVersion(versionKey).Value == "")
                {
                    LocalVerions.AddVersion(new Objects.Version() { Key = versionKey, Value = "0.0.0" });
                    return true;
                }

                if (!System.IO.File.Exists(localVersionFilePath))
                {
                    LocalVerions.AddVersion(new Objects.Version() { Key = versionKey, Value = "0.0.0" });
                    return true;
                }

                if (string.IsNullOrEmpty(ReadLocalVersion(versionKey)) || string.IsNullOrWhiteSpace(ReadLocalVersion(versionKey)))
                {
                    LocalVerions.AddVersion(new Objects.Version() { Key = versionKey, Value = "0.0.0" });
                    needsUpdate = true;
                }

                int currentRelease = ParseVersioning(ReadLocalVersion(versionKey))[0], currentMajor = ParseVersioning(ReadLocalVersion(versionKey))[1], currentMinor = ParseVersioning(ReadLocalVersion(versionKey))[2];
                int remoteRelease = ParseVersioning(ReadRemoteVersion(remoteVersionFilePath, versionKey))[0], remoteMajor = ParseVersioning(ReadRemoteVersion(remoteVersionFilePath, versionKey))[1], remoteMinor = ParseVersioning(ReadRemoteVersion(remoteVersionFilePath, versionKey))[2];

                int RemoteUnPartitioned = int.Parse($"{remoteRelease}{remoteMajor}{remoteMinor}"), CurrentUnPartitioned = int.Parse($"{currentRelease}{currentMajor}{currentMinor}");

                if (CurrentUnPartitioned < RemoteUnPartitioned)
                {
                    return true;
                }
            }
            catch
            {
                needsUpdate = true;
            }

            return needsUpdate;
        }

        /// <summary>
        /// Get Archive Download URL Stored in the Remote Verion File
        /// </summary>
        /// <param name="url_key"> </param>
        /// <returns> </returns>
        public string GetArchiveURL(string url_key)
        {
            return RemoteVersions.VersionManager.GetConfigByKey(url_key).Value;
        }

        public string GetChangeLog(string path, string changelog_key = "changelog")
        {
            string f = Path.Combine(path, "CHANGELOG");
            if (!File.Exists(f))
            {
                f = GenerateChangelog(path, changelog_key);
            }

            using (StreamReader reader = new StreamReader(f))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Get Executable Name Stored in the Remote Verion File
        /// </summary>
        /// <param name="exe_key"> </param>
        /// <returns> </returns>
        public string GetExecutableName(string exe_key)
        {
            return RemoteVersions.VersionManager.GetConfigByKey(exe_key).Value;
        }

        /// <summary>
        /// Run Before Anything Else
        /// </summary>
        /// <param name="localVersionFilePath"> </param>
        /// <param name="versionURL">           </param>
        public void Init(string versionURL, string localVersionFilePath)
        {
            string remote_path = Path.Combine(Environment.GetEnvironmentVariable("TMP"), "Remote Version");
            if (File.Exists(remote_path))
            {
                File.Delete(remote_path);
            }

            LocalVerions = new Versions(localVersionFilePath);
            RemoteVersions = new Versions(remote_path);
            LocalVersionPath = localVersionFilePath;
            DownloadRemoteVersion(versionURL);
            AppDomain.CurrentDomain.ProcessExit += ((object sender, EventArgs args) =>
            {
                if (File.Exists(RemoteVersions.Path))
                {
                    File.Delete(RemoteVersions.Path);
                }
            });
        }

        /// <summary>
        /// Automatically Checks for Update and Downloads if Needed
        /// <para> Version Files Should be Orginized Like so </para>
        /// <code>Version Key 1.0.0 </code>
        /// </summary>
        /// <param name="versionKey">            The Label before the Version Numbers </param>
        /// <param name="localVersionFilePath"> 
        /// The Path to the current Version File, If not found it will automatically update
        /// </param>
        /// <param name="remoteVersionFilePath"> The URL to the updated Version File </param>
        /// <param name="zipUrl">               
        /// The Direct Download URL to the Update Zip Archive
        /// </param>
        /// <param name="zipDirectory">         
        /// The Temp Directory where the Zip Archive will be Downloaded
        /// </param>
        /// <param name="unzipDirectory">        The Application Directory </param>
        /// <param name="launchExecutableName">  The Application Executable Path </param>
        /// <param name="overwrite">            
        /// Weather to Clear the Application Directory Prior to Unziping
        /// </param>
        public void Update(string versionKey, string localVersionFilePath, string remoteVersionFilePath, string zipUrl, string zipDirectory, string unzipDirectory, string launchExecutableName, bool overwrite = true)
        {
            if (LocalVersionPath == "")
            {
                Init(remoteVersionFilePath, localVersionFilePath);
            }

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
        /// <param name="key"> </param>
        public void UpdateVersionFile(string key)
        {
            LocalVerions.UpdateVersion(key, RemoteVersions.GetVersion(key).Value);
        }

        #endregion Public Methods

        #region Private Methods

        private void DownloadRemoteVersion(string url)
        {
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                client.DownloadFile(url, RemoteVersions.Path);
                RemoteVersions.VersionManager.FindPreExistingConfigs();
                client.Dispose();
            }
        }

        private string GenerateChangelog(string path, string changelog_key = "changelog")
        {
            using (StreamWriter writer = new StreamWriter(Path.Combine(path, "CHANGELOG")))
            {
                writer.WriteLine(LocalVerions.GetChangeLog(changelog_key));
                writer.Flush();
                writer.Dispose();
                writer.Close();
            }
            return Path.Combine(path, "CHANGELOG");
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

        private string ReadLocalVersion(string key)
        {
            if (LocalVerions.GetVersion(key) == null)
            {
                LocalVerions.AddVersion(new Objects.Version() { Key = key, Value = "0.0.0" });
            }
            return LocalVerions.GetVersion(key).Value;
        }

        private string ReadRemoteVersion(string url, string key)
        {
            string value = "";
            if (RemoteVersions.GetVersion(key) != null)
            {
                value = RemoteVersions.GetVersion(key).Value;
            }
            else
            {
                RemoteVersions.AddVersion(new Objects.Version() { Key = key, Value = "0.0.0" });
            }
            return value;
        }

        #endregion Private Methods
    }
}