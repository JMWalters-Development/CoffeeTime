using System;
using System.IO;
using System.Reactive.Linq;
using CoffeeTime.Modules.DirectoryMonitor.Enums;
using CoffeeTime.Modules.DirectoryMonitor.Interfaces;
using CoffeeTime.Modules.DirectoryMonitor.Records;

namespace CoffeeTime.Modules.DirectoryMonitor.Services;

public class DirectoryMonitor : IDirectoryMonitor, IDisposable
{
    #region Private fields and properties

    private bool _disposed;
    private readonly FileSystemWatcher _fileSystemWatcher;
    private readonly IObservable<DirectoryActivity> _fileSystemChanges;

    #endregion
    
    #region Public fields and properties
    
    public bool EnableRaisingEvents
    {
        get => _fileSystemWatcher.EnableRaisingEvents;
        set => _fileSystemWatcher.EnableRaisingEvents = value;
    }
    public IObservable<DirectoryActivity> FileSystemChanges => _fileSystemChanges;
    public string Filter
    {
        get => _fileSystemWatcher.Filter;
        set => _fileSystemWatcher.Filter = value;
    }
    public bool IncludeSubdirectories
    {
        get => _fileSystemWatcher.IncludeSubdirectories;
        set => _fileSystemWatcher.IncludeSubdirectories = value;
    }
    public NotifyFilters NotifyFilter
    {
        get => _fileSystemWatcher.NotifyFilter;
        set => _fileSystemWatcher.NotifyFilter = value;
    }
    public string Path => _fileSystemWatcher.Path;
    
    #endregion

    public DirectoryMonitor(string path)
    {
        _fileSystemWatcher = new FileSystemWatcher(path);

        var changed = Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
            h => _fileSystemWatcher.Changed += h,
            h => _fileSystemWatcher.Changed -= h
        ).Select(e => new DirectoryActivity(DirectoryActivityType.Changed, e.EventArgs.FullPath));
        
        var created = Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
            h => _fileSystemWatcher.Created += h,
            h => _fileSystemWatcher.Created -= h
        ).Select(e => new DirectoryActivity(DirectoryActivityType.Created, e.EventArgs.FullPath));

        var deleted = Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
            h => _fileSystemWatcher.Deleted += h,
            h => _fileSystemWatcher.Deleted -= h
        ).Select(e => new DirectoryActivity(DirectoryActivityType.Deleted, e.EventArgs.FullPath));

        var renamed = Observable.FromEventPattern<RenamedEventHandler, RenamedEventArgs>(
            h => _fileSystemWatcher.Renamed += h,
            h => _fileSystemWatcher.Renamed -= h
        ).Select(e => new DirectoryActivity(
            DirectoryActivityType.Renamed,
            e.EventArgs.FullPath,
            e.EventArgs.OldFullPath
        ));
        
        _fileSystemChanges = Observable.Merge(changed, created, deleted, renamed);
    }

    public void Dispose()
    {
        if (_disposed) return;
        
        _disposed = true;
        _fileSystemWatcher.Dispose();
        GC.SuppressFinalize(this);
    }
}