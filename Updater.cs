
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
        string url, zipdir, unzipdir, lexe;
        bool overwrite;
        int progress = 0;
        public string URL => url;

        public string ZipDirectory => zipdir;

        public string UnzipDirectory => unzipdir;

        public string LaunchExecutableName => lexe;

        public bool OverwriteDirectory => overwrite;

        public int DownloadProgress => progress;

        Updater(string _url, string _zipDirectory, string _unzipDirectory, string _launchExecutableName, bool _overwrite)
        {
            url = _url;
            zipdir = _zipDirectory;
            unzipdir = _unzipDirectory;
            lexe = _launchExecutableName;
            overwrite = _overwrite;
            if (!System.IO.Directory.Exists(UnzipDirectory)) System.IO.Directory.CreateDirectory(UnzipDirectory);
            if (!System.IO.Directory.Exists(ZipDirectory)) System.IO.Directory.CreateDirectory(ZipDirectory);
            if (System.IO.File.Exists(System.IO.Path.Combine(ZipDirectory, "unzip.zip"))) System.IO.File.Delete(System.IO.Path.Combine(ZipDirectory, "unzip.zip"));

            Start();
        }

        public static Interfaces.IUpdater Init(string _url, string _zipDirectory, string _unzipDirectory, string _launchExecutableName, bool _overwrite)
        {
            return new Updater(_url, _zipDirectory, _unzipDirectory, _launchExecutableName, _overwrite);
        }

        void Start()
        {
            Download();
        }

        public void Download()
        {
            using (var client = new System.Net.WebClient())
            {
                client.DownloadFile(URL, System.IO.Path.Combine(ZipDirectory, "unzip.zip"));
                client.DownloadProgressChanged += ((object sender, System.Net.DownloadProgressChangedEventArgs e) => { progress = e.ProgressPercentage; });
                client.DownloadFileCompleted += ((object sender, System.ComponentModel.AsyncCompletedEventArgs e) => { Unzip(); });
            }
        }

        public void Unzip()
        {
            if (OverwriteDirectory)
            {
                foreach (var file in System.IO.Directory.GetFiles(UnzipDirectory))
                {
                    System.IO.File.Delete(file);
                }
            }
            System.IO.Compression.ZipFile.ExtractToDirectory(System.IO.Path.Combine(ZipDirectory, "update.zip"), UnzipDirectory);
            CleanUp();
        }

        public void CleanUp()
        {
            if (System.IO.File.Exists(System.IO.Path.Combine(ZipDirectory, "unzip.zip"))) System.IO.File.Delete(System.IO.Path.Combine(ZipDirectory, "unzip.zip"));
            if (System.IO.Directory.Exists(ZipDirectory)) System.IO.Directory.Delete(ZipDirectory);
            LaunchExecutable();
        }

        public void LaunchExecutable()
        {
            if (new System.Diagnostics.Process() { StartInfo = new System.Diagnostics.ProcessStartInfo() { FileName = LaunchExecutableName, CreateNoWindow = true, UseShellExecute = false } }.Start())
                System.Environment.Exit(0);
        }

    }
}
