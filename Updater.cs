
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
        public string URL => url;

        public string ZipDirectory => zipdir;

        public string UnzipDirectory => unzipdir;

        public string LaunchExecutableName => lexe;

        public bool OverwriteDirectory => overwrite;

        public int DownloadProgress => progress;

        private Updater(string _url, string _zipDirectory, string _unzipDirectory, string _launchExecutableName, bool _overwrite)
        {
            url = _url;
            zipdir = _zipDirectory;
            unzipdir = _unzipDirectory;
            lexe = _launchExecutableName;
            overwrite = _overwrite; 
            
            //if (OverwriteDirectory)
            //{
            //    foreach (string file in System.IO.Directory.GetFiles(UnzipDirectory, "*", System.IO.SearchOption.AllDirectories))
            //    {
            //        System.IO.File.Delete(file);
            //    }
            //}

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

            //Start();
        }

        public static Interfaces.IUpdater Init(string _url, string _zipDirectory, string _unzipDirectory, string _launchExecutableName, bool _overwrite)
        {
            System.Console.WriteLine("Initializing Updater...");
            return new Updater(_url, _zipDirectory, _unzipDirectory, _launchExecutableName, _overwrite);
        }

        private void Start()
        {
            System.Console.WriteLine("Starting Updater...");
            //Download();
        }

        public void Download()
        {
            System.Console.WriteLine("Downloading Update...");
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                //client.DownloadFileCompleted += ((object sender, System.ComponentModel.AsyncCompletedEventArgs e) => { Unzip(); });
                client.DownloadProgressChanged += ((object sender, System.Net.DownloadProgressChangedEventArgs e) => { System.Console.WriteLine($"{e.ProgressPercentage}%"); progress = e.ProgressPercentage; });
                client.DownloadFile(URL, System.IO.Path.Combine(ZipDirectory, "update.zip"));
                client.Dispose();
                System.Threading.Thread.Sleep(2 * 1000);
                //Unzip();
            }
        }

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
            //CleanUp();
        }

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
