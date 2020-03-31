
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
        public static void Update(string versionKey, string localVersionFilePath, string remoteVersionFilePath, string zipUrl, string zipDirectory, string unzipDirectory, string launchExecutableName, bool overwrite)
        {
            if (CheckForUpdate(versionKey, localVersionFilePath, remoteVersionFilePath) || !System.IO.File.Exists(System.IO.Path.Combine(unzipDirectory, launchExecutableName)))
            {
                System.Console.WriteLine("Update Needed!");
                var update = Updater.Init(zipUrl, zipDirectory, unzipDirectory, launchExecutableName, overwrite);
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

        public static bool CheckForUpdate(string versionKey, string localVersionFilePath, string remoteVersionFilePath)
        {
            int currentRelease = ParseVersioning(ReadLocalVersion(localVersionFilePath, versionKey))[0], currentMajor = ParseVersioning(ReadLocalVersion(localVersionFilePath, versionKey))[1], currentMinor = ParseVersioning(ReadLocalVersion(localVersionFilePath, versionKey))[2];
            int remoteRelease = ParseVersioning(ReadRemoteVersion(remoteVersionFilePath, versionKey))[0], remoteMajor = ParseVersioning(ReadRemoteVersion(remoteVersionFilePath, versionKey))[1], remoteMinor = ParseVersioning(ReadRemoteVersion(remoteVersionFilePath, versionKey))[2];

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
