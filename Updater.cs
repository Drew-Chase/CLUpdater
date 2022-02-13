using System.Threading.Tasks;

namespace ChaseLabs.CLUpdate
{
    /// <summary>
    /// <para> Author: Drew Chase </para>
    /// <para> Company: Chase Labs </para>
    /// </summary>
    public class Updater : Interfaces.IUpdater
    {
        #region Private Fields

        private readonly bool overwrite;

        private readonly int progress = 0;

        private readonly string url, zipdir, unzipdir, lexe;

        #endregion Private Fields

        #region Private Constructors

        private Updater(string _url, string _zipDirectory, string _unzipDirectory, string _launchExecutableName, bool _overwrite)
        {
            url = _url;
            zipdir = _zipDirectory;
            unzipdir = _unzipDirectory;
            lexe = _launchExecutableName;
            overwrite = _overwrite;

            if (System.IO.File.Exists(System.IO.Path.Combine(ZipDirectory, "update.zip")))
            {
                System.Console.WriteLine("Removing Spent Update Archive");
                System.IO.File.Delete(System.IO.Path.Combine(ZipDirectory, "update.zip"));
            }
        }

        #endregion Private Constructors

        #region Public Properties

        /// <summary>
        /// Returns the Current Download Client
        /// </summary>
        public System.Net.WebClient DownloadClient { get; private set; }

        /// <summary>
        /// The Executable to be Launched apon Completion
        /// </summary>
        public string LaunchExecutableName => lexe;

        /// <summary>
        /// Returns Weather or not to remove all files in a directory or just skip them
        /// </summary>
        public bool OverwriteDirectory => overwrite;

        /// <summary>
        /// The Directory in Which the Application File will Reside
        /// </summary>
        public string UnzipDirectory => unzipdir;

        /// <summary>
        /// Returns the Download URL
        /// </summary>
        public string URL => url;

        /// <summary>
        /// The Temp Directory in Which the Zip File will Reside
        /// </summary>
        public string ZipDirectory => zipdir;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// A Static Method to Initialize the Updater
        /// </summary>
        /// <param name="_url">                 
        /// The Direct Download URL of the Zip File
        /// </param>
        /// <param name="_zipDirectory">        
        /// The Temp Directory to Download the Zip Archive
        /// </param>
        /// <param name="_unzipDirectory">       The Application Directory </param>
        /// <param name="_launchExecutableName"> The Application Executable to Launch Apon Completion </param>
        /// <param name="_overwrite">           
        /// Weather to Clear the Application Directory Prior to Unziping
        /// </param>
        /// <returns> </returns>
        public static Updater Init(string _url, string _zipDirectory, string _unzipDirectory, string _launchExecutableName, bool _overwrite = true)
        {
            System.Console.WriteLine("Initializing Updater...");
            return new Updater(_url, _zipDirectory, _unzipDirectory, _launchExecutableName, _overwrite);
        }

        /// <summary>
        /// Removes the Zip Archive and Temp Directory
        /// </summary>
        public void CleanUp()
        {
            System.Console.WriteLine("Cleaning Up...");
            if (System.IO.File.Exists(System.IO.Path.Combine(ZipDirectory, "update.zip")))
            {
                System.IO.File.Delete(System.IO.Path.Combine(ZipDirectory, "update.zip"));
            }

            if (System.IO.Directory.Exists(ZipDirectory))
            {
                System.IO.Directory.Delete(ZipDirectory);
            }
        }

        /// <summary>
        /// Downloads the Zip Archive From URL
        /// </summary>
        public void Download()
        {
            if (!System.IO.Directory.Exists(ZipDirectory))
            {
                System.Console.WriteLine("Creating Application Directory");
                System.IO.Directory.CreateDirectory(ZipDirectory);
            }
            System.Console.WriteLine("Downloading Update...");
            Task.Run(() => Utilities.Downloader.Downloader.DownloadAsync(URL, "update.zip", /*System.IO.Path.Combine(ZipDirectory, "update.zip")*/ZipDirectory, 999999)).Wait();
            System.Threading.Thread.Sleep(2 * 1000);

            //using (DownloadClient = new System.Net.WebClient())
            //{
            //    int percentage = 0;
            //    //await DownloadClient.DownloadFileAsync(new System.Uri(URL), System.IO.Path.Combine(ZipDirectory, "update.zip"));
            //    System.Threading.Tasks.Task tsk = DownloadClient.DownloadFileTaskAsync(URL, System.IO.Path.Combine(ZipDirectory, "update.zip"));
            //    //DownloadClient.DownloadProgressChanged += ((object sender, System.Net.DownloadProgressChangedEventArgs e) => System.Console.WriteLine($"Dowloading: {e.ProgressPercentage}%"));
            //    DownloadClient.DownloadProgressChanged += (s, e) =>
            //    {
            //        if (e.ProgressPercentage > percentage)
            //        {
            //            System.Console.WriteLine($"Downloading: {e.ProgressPercentage}%");
            //            percentage = e.ProgressPercentage;
            //        }
            //    };
            //    DownloadClient.Dispose();
            //    System.Threading.Thread.Sleep(2 * 1000);
            //    tsk.Wait();
            //}
        }

        /// <summary>
        /// Launches the Applications Executable
        /// </summary>
        public void LaunchExecutable()
        {
            System.Threading.Thread.Sleep(2 * 1000);
            System.Console.WriteLine("Launching Executable...");
            if (new System.Diagnostics.Process() { StartInfo = new System.Diagnostics.ProcessStartInfo() { FileName = System.IO.Path.Combine(UnzipDirectory, LaunchExecutableName) } }.Start())
            {
                System.Environment.Exit(0);
            }
        }

        /// <summary>
        /// Unzips the Zip Archive to Application Directory
        /// </summary>
        public void Unzip()
        {
            System.Console.WriteLine("Unziping Update...");
            if (OverwriteDirectory)
            {
                if (System.IO.Directory.Exists(UnzipDirectory))
                {
                    System.IO.Directory.Delete(UnzipDirectory, true);
                }

                System.IO.Directory.CreateDirectory(UnzipDirectory);
            }
            System.IO.Compression.ZipFile.ExtractToDirectory(System.IO.Path.Combine(ZipDirectory, "update.zip"), UnzipDirectory);
            System.Threading.Thread.Sleep(2 * 1000);
        }

        #endregion Public Methods

        #region Private Methods

        private void Start()
        {
            System.Console.WriteLine("Starting Updater...");
        }

        #endregion Private Methods
    }
}