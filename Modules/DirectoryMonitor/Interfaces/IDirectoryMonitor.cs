using System;
using System.IO;
using CoffeeTime.Modules.DirectoryMonitor.Records;

namespace CoffeeTime.Modules.DirectoryMonitor.Interfaces;

public interface IDirectoryMonitor
{
    bool EnableRaisingEvents { get; set; }
    IObservable<DirectoryActivity> FileSystemChanges { get; }
    string Filter { get; set; }
    bool IncludeSubdirectories { get; set; }
    NotifyFilters NotifyFilter { get; set; }
}