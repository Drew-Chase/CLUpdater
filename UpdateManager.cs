using System;
using System.Collections.Generic;
using System.Text;

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
        public static bool CheckForUpdate(string versionKey, string localVersionFilePath, string remoteVersionFilePath)
        {
            int currentRelease = ParseVersioning(ReadLocalVersion(localVersionFilePath, versionKey))[0], currentMajor = ParseVersioning(ReadLocalVersion(localVersionFilePath, versionKey))[1], currentMinor = ParseVersioning(ReadLocalVersion(localVersionFilePath, versionKey))[2];
            int remoteRelease = ParseVersioning(ReadRemoteVersion(remoteVersionFilePath, versionKey))[0], remoteMajor = ParseVersioning(ReadRemoteVersion(remoteVersionFilePath, versionKey))[1], remoteMinor = ParseVersioning(ReadRemoteVersion(remoteVersionFilePath, versionKey))[2];

            if (currentRelease < remoteRelease) return true;
            if (currentMajor < remoteMajor) return true;
            if (currentMinor < remoteMinor) return true;

            return false;
        }

        static int[] ParseVersioning(string version)
        {
            int[] value = { 0, 0, 0 };
            for (int i = 0; i < version.Split('.').Length; i++)
            {
                if (i == 0) int.TryParse(version.Split('.')[i], out value[i]);
                if (i == 1) int.TryParse(version.Split('.')[i], out value[i]);
                if (i == 2) int.TryParse(version.Split('.')[i], out value[i]);
            }
            return value;
        }

        static string ReadRemoteVersion(string url, string key)
        {
            string value = string.Empty;
            using (var client = new System.Net.WebClient())
            {
                value = client.DownloadString(url);
                foreach (string s in value.Split(Environment.NewLine.ToCharArray()[0]))
                {
                    if (s.StartsWith(key))
                        value = s.Replace(key, "");
                }
            }
            return value;
        }

        static string ReadLocalVersion(string path, string key)
        {
            string value = string.Empty;
            using (var reader = new System.IO.StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    string txt = reader.ReadLine();
                    if (txt.StartsWith(key)) value = txt.Replace(key, "");
                }
            }
            return value;
        }

    }
}
