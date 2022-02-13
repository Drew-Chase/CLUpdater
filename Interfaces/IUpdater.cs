namespace ChaseLabs.CLUpdate.Interfaces
{
    /// <summary>
    /// The Interface Class for the Updater
    /// </summary>
    public interface IUpdater
    {
        #region Public Properties

        /// <summary>
        /// The Executable to be Launched apon Completion
        /// </summary>
        string LaunchExecutableName { get; }

        /// <summary>
        /// Returns Weather or not to remove all files in a directory or just skip them
        /// </summary>
        bool OverwriteDirectory { get; }

        /// <summary>
        /// The Directory in Which the Application File will Reside
        /// </summary>
        string UnzipDirectory { get; }

        /// <summary>
        /// Returns the Download URL
        /// </summary>
        string URL { get; }

        /// <summary>
        /// The Temp Directory in Which the Zip File will Reside
        /// </summary>
        string ZipDirectory { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Removes the Zip Archive and Temp Directory
        /// </summary>
        void CleanUp();

        /// <summary>
        /// Downloads the Zip Archive From URL
        /// </summary>
        void Download();

        /// <summary>
        /// Launches the Applications Executable
        /// </summary>
        void LaunchExecutable();

        /// <summary>
        /// Unzips the Zip Archive to Application Directory
        /// </summary>
        void Unzip();

        #endregion Public Methods
    }
}