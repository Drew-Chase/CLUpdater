# Chase Labs Auto Update Wiki
1. [Installation](https://github.com/DcmanProductions/CLUpdater#install)
1. [Getting Started](https://github.com/DcmanProductions/CLUpdater#getting-started)
1. [Checking For Update](https://github.com/DcmanProductions/CLUpdater/#how-to-check-for-updates)
1. [Downloading Update](https://github.com/DcmanProductions/CLUpdater#downloading-update)

# Chase Labs Auto Update Utility
An Easy to Use Update Utility

## Install
### With Package Manager
`Install-Package ChaseLabs.Updater`
### With Nuget Manageer
`ChaseLabs.Updater` and Install Latest

## For Video Tutorial
[Visit](https://youtu.be/HDLHdJC3sLc) Our Youtube Video on the Subject

## Getting Started
Create a Variable for the Application Zip Direct Download 
```csharp
string url
```

Create a Variable for the Version File Direct Download 
```csharp 
string remote_version_url
```

Create a Variable for the Version Key 
```csharp 
string version_key
```
this is used to determine what version the application should be considering

Create a Variable for the Zip Temp Download Path 
```csharp
string temp_path
```

Create a Variable for the Application Path
```csharp
string application_path
```
this will be where the zip archive will be unzipped

Create a Variable for the Launch Executable
```csharp 
string launch_exe
```
this will be the application that will run after the update has completed.



Combine That in the Init Method like so 
```csharp
IUpdater update = Updater.Init(url, temp_path, application_path, launch_exe)
```

## How to Check for Updates
Use
```csharp
UpdateManager.CheckForUpdate(Version Key, "Path/to/Version/Key", "https://Version.Key/Direct-Download")
```
If it returns `true` there is an update pending,<br><br>
if it returns `false` than the application is up-to-date

## Downloading Update
To Download the Zip File use
```csharp
Download();
```
To Unzip the Update use
```csharp 
Unzip();
```
To Remove all Temporary Files and Directories use 
```csharp
CleanUp(); 
```
To Launch the Application use 
```csharp
LaunchExecutable()
```
