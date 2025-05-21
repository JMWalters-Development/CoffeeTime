using System;
using System.IO;
using System.Reactive.Linq;
using CoffeeTime.Modules.DirectoryMonitors.Enums;
using CoffeeTime.Modules.DirectoryMonitors.Interfaces;
using CoffeeTime.Modules.DirectoryMonitors.Records;

namespace CoffeeTime.Modules.DirectoryMonitors.Services;

public class DirectoryMonitor : IDirectoryMonitor, IDisposable
{
    #region Private fields and properties

    private bool _disposed;
    private readonly FileSystemWatcher _fileSystemWatcher;

    #endregion
    
    #region Public fields and properties
    
    public bool EnableRaisingEvents
    {
        get => _fileSystemWatcher.EnableRaisingEvents;
        set => _fileSystemWatcher.EnableRaisingEvents = value;
    }
    public IObservable<DirectoryActivity> FileSystemChanges { get; }
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
        ).Select(e => new DirectoryActivity(
            DirectoryActivityType.Changed,
            e.EventArgs.FullPath,
            e.EventArgs.Name));
        
        var created = Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
            h => _fileSystemWatcher.Created += h,
            h => _fileSystemWatcher.Created -= h
        ).Select(e => new DirectoryActivity(
            DirectoryActivityType.Created,
            e.EventArgs.FullPath,
            e.EventArgs.Name));

        var deleted = Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
            h => _fileSystemWatcher.Deleted += h,
            h => _fileSystemWatcher.Deleted -= h
        ).Select(e => new DirectoryActivity(
            DirectoryActivityType.Deleted,
            e.EventArgs.FullPath,
            e.EventArgs.Name));

        var renamed = Observable.FromEventPattern<RenamedEventHandler, RenamedEventArgs>(
            h => _fileSystemWatcher.Renamed += h,
            h => _fileSystemWatcher.Renamed -= h
        ).Select(e => new DirectoryActivity(
            DirectoryActivityType.Renamed,
            e.EventArgs.FullPath,
            e.EventArgs.Name,
            e.EventArgs.OldFullPath,
            e.EventArgs.OldName
        ));
        
        FileSystemChanges = Observable.Merge(changed, created, deleted, renamed);
    }
    
    #region Public functions and methods

    public void Dispose()
    {
        if (_disposed) return;
        
        _disposed = true;
        _fileSystemWatcher.Dispose();
        GC.SuppressFinalize(this);
    }
    
    #endregion
}