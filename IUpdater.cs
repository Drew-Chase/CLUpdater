using System;
using System.Collections.Generic;
using System.Text;

namespace ChaseLabs.CLUpdate
{
    public interface IUpdater
    {
        string URL { get; }
        string ZipDirectory { get; }
        string UnzipDirectory { get; }
        string LaunchExecutableName { get; }
        bool OverwriteDirectory { get; }
        int DownloadProgress { get; }

        void Download();
        void Unzip();
        void CleanUp();
        void LaunchExecutable();
    }

}
