# Chase Labs Auto Update Wiki
1. [Installation](https://github.com/DcmanProductions/CLUpdater#install)
1. [Getting Started](https://github.com/DcmanProductions/CLUpdater#getting-started)
1. [Checking For Update](https://github.com/DcmanProductions/CLUpdater/#how-to-check-for-updates)
1. [Downloading Update](https://github.com/DcmanProductions/CLUpdater#downloading-update)

# Chase Labs Auto Update Utility
An Easy to Use Update Utility

## Install
### With Package Manager
`Install-Package ChaseLabs.Updater -Version 0.0.9`
### With Nuget Manageer
`ChaseLabs.Updater` and Install Latest

## For Video Tutorial
[Visit](https://youtu.be/HDLHdJC3sLc) Our Youtube Video on the Subject

## Getting Started
Create a Variable for the Application Zip Direct Download `string url`

Create a Variable for the Version File Direct Download `string remote_version_url`

Create a Variable for the Version Key `string version_key` this is used to determine what version the application should be considering

Create a Variable for the Zip Temp Download Path `string temp_path`

Create a Variable for the Application Path `string application_path` this will be where the zip archive will be unzipped

Create a Variable for the Launch Executable `string launch_exe` this will be the application that will run after the update has completed.


Combine That in the Init Method like so `IUpdater update = Updater.Init(url, temp_path, application_path, launch_exe)`

## How to Check for Updates
Use<br><br>
`UpdateManager.CheckForUpdate(Version Key, Path to Version Key, Version Key Direct Download)`<br><br>
If it returns `true` there is an update pending,<br><br>
if it returns `false` than the application is up-to-date

## Downloading Update
To Download the Zip File use the `Download()` Method<br>
To Unzip the Update use the `Unzip()` Method<br>
To Remove all Temporary Files and Directories use the `CleanUp()` Method<br>
To Launch the Application use the `LaunchExecutable()` Method<br>