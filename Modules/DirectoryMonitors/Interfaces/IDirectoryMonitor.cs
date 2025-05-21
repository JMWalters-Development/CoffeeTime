using System;
using System.IO;
using CoffeeTime.Modules.DirectoryMonitors.Records;

namespace CoffeeTime.Modules.DirectoryMonitors.Interfaces;

public interface IDirectoryMonitor
{
    bool EnableRaisingEvents { get; set; }
    IObservable<DirectoryActivity> FileSystemChanges { get; }
    string Filter { get; set; }
    bool IncludeSubdirectories { get; set; }
    NotifyFilters NotifyFilter { get; set; }
    string Path { get; }
}