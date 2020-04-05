
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
    public class Updater : Interfaces.IUpdater
    {
        private readonly string url, zipdir, unzipdir, lexe;
        private readonly bool overwrite;
        private int progress = 0;
        /// <summary>
        /// Returns the Download URL
        /// </summary>
        public string URL => url;

        /// <summary>
        /// The Temp Directory in Which the Zip File will Reside
        /// </summary>
        public string ZipDirectory => zipdir;

        /// <summary>
        /// The Directory in Which the Application File will Reside
        /// </summary>
        public string UnzipDirectory => unzipdir;

        /// <summary>
        /// The Executable to be Launched apon Completion
        /// </summary>
        public string LaunchExecutableName => lexe;

        /// <summary>
        /// Returns Weather or not to remove all files in a directory or just skip them
        /// </summary>
        public bool OverwriteDirectory => overwrite;
        /// <summary>
        /// Returns the Current Download Progress in interger form
        /// </summary>
        public int DownloadProgress => progress;

        private Updater(string _url, string _zipDirectory, string _unzipDirectory, string _launchExecutableName, bool _overwrite)
        {
            url = _url;
            zipdir = _zipDirectory;
            unzipdir = _unzipDirectory;
            lexe = _launchExecutableName;
            overwrite = _overwrite;

            if (!System.IO.Directory.Exists(UnzipDirectory))
            {
                System.Console.WriteLine("Removing Spent Update Archive Directory");
                System.IO.Directory.CreateDirectory(UnzipDirectory);
            }

            if (!System.IO.Directory.Exists(ZipDirectory))
            {
                System.Console.WriteLine("Creating Application Directory");
                System.IO.Directory.CreateDirectory(ZipDirectory);
            }

            if (System.IO.File.Exists(System.IO.Path.Combine(ZipDirectory, "update.zip")))
            {
                System.Console.WriteLine("Removing Spent Update Archive");
                System.IO.File.Delete(System.IO.Path.Combine(ZipDirectory, "update.zip"));
            }

        }

        /// <summary>
        /// A Static Method to Initialize the Updater
        /// </summary>
        /// <param name="_url">The Direct Download URL of the Zip File</param>
        /// <param name="_zipDirectory">The Temp Directory to Download the Zip Archive</param>
        /// <param name="_unzipDirectory">The Application Directory</param>
        /// <param name="_launchExecutableName">The Application Executable to Launch Apon Completion</param>
        /// <param name="_overwrite">Weather to Clear the Application Directory Prior to Unziping</param>
        /// <returns></returns>
        public static Interfaces.IUpdater Init(string _url, string _zipDirectory, string _unzipDirectory, string _launchExecutableName, bool _overwrite = true)
        {
            System.Console.WriteLine("Initializing Updater...");
            return new Updater(_url, _zipDirectory, _unzipDirectory, _launchExecutableName, _overwrite);
        }

        private void Start()
        {
            System.Console.WriteLine("Starting Updater...");
        }

        /// <summary>
        /// Downloads the Zip Archive From URL
        /// </summary>
        public void Download()
        {
            System.Console.WriteLine("Downloading Update...");
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                client.DownloadProgressChanged += ((object sender, System.Net.DownloadProgressChangedEventArgs e) => { System.Console.WriteLine($"{e.ProgressPercentage}%"); progress = e.ProgressPercentage; });
                client.DownloadFile(URL, System.IO.Path.Combine(ZipDirectory, "update.zip"));
                client.Dispose();
                System.Threading.Thread.Sleep(2 * 1000);
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
                foreach (string file in System.IO.Directory.GetFiles(UnzipDirectory, "*", System.IO.SearchOption.AllDirectories))
                {
                    System.IO.File.Delete(file);
                }
            }
            System.IO.Compression.ZipFile.ExtractToDirectory(System.IO.Path.Combine(ZipDirectory, "update.zip"), UnzipDirectory);
            System.Threading.Thread.Sleep(2 * 1000);
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

    }
}
