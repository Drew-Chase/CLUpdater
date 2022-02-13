using ChaseLabs.CLConfiguration.List;

namespace ChaseLabs.CLUpdate.Lists
{
    /// <summary>
    /// <para>
    /// Author: Drew Chase
    /// </para>
    /// <para>
    /// Company: Chase Labs
    /// </para>
    /// </summary>
    public class Versions
    {
        /// <summary>
        /// The Config Manager Object of the Current Version File
        /// </summary>
        public ConfigManager VersionManager;

        private readonly string _path;
        public string Path => _path;

        /// <summary>
        /// Initializes the Version File in the Path
        /// </summary>
        /// <param name="VersionFilePath"></param>
        public Versions(string VersionFilePath)
        {
            _path = VersionFilePath;
            VersionManager = new ConfigManager(VersionFilePath);
        }

        /// <summary>
        /// Adds a Version Object to the Version File.
        /// </summary>
        /// <param name="version"></param>
        public void AddVersion(Objects.Version version)
        {
            VersionManager.Add(version.Key, version.Value);
        }

        /// <summary>
        /// Adds a Version using a Key and Value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddVersion(string key, string value)
        {
            VersionManager.Add(key, value);
        }

        /// <summary>
        /// Gets the Version based on the Key
        /// <para>Returns Null if Not Found</para>
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Version Object</returns>
        public Objects.Version GetVersion(string key)
        {
            if (VersionManager.GetConfigByKey(key) != null)
            {
                return new Objects.Version() { Key = key, Value = VersionManager.GetConfigByKey(key).Value };
            }

            return new Objects.Version() { Key = key, Value = "0.0.0" }; ;
        }

        /// <summary>
        /// Gets the Version Changelog
        /// <para>Returns Empty String If Not Found</para>
        /// </summary>
        /// <param name="key">Optional</param>
        /// <returns>Full Changelog</returns>
        public string GetChangeLog(string key = "changelog")
        {
            if (VersionManager.GetConfigByKey(key) != null)
            {
                return VersionManager.GetConfigByKey(key).Value;
            }

            return "";
        }

        /// <summary>
        /// Removes the Version with specified Key
        /// </summary>
        /// <param name="key"></param>
        public void RemoveVersion(string key)
        {
            VersionManager.Remove(key);
        }

        /// <summary>
        /// Updates the Versions Value based on specified Key
        /// <para>Adds a New Version with Key and Value if one did not previously exist</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void UpdateVersion(string key, string value)
        {
            if (VersionManager.GetConfigByKey(key) != null)
            {
                VersionManager.GetConfigByKey(key).Value = value;
            }
            else
            {
                AddVersion(new Objects.Version() { Key = key, Value = value });
            }
        }
    }
}