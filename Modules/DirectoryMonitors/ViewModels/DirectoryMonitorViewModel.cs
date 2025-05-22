using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using CoffeeTime.Modules.DirectoryMonitors.Enums;
using CoffeeTime.Modules.DirectoryMonitors.Records;
using CoffeeTime.Modules.DirectoryMonitors.Services;
using CoffeeTime.ViewModels;
using ReactiveUI;

namespace CoffeeTime.Modules.DirectoryMonitors.ViewModels;

public class DirectoryMonitorViewModel : ViewModelBase, IDisposable
{
    #region Private fields and properties
    
    private readonly DirectoryMonitor _directoryMonitor;
    private string _filter = string.Empty;
    private bool _monitoringChanged;
    private bool _monitoringCreated;
    private bool _monitoringDeleted;
    private bool _monitoringRenamed;
    private string _mostRecentLog = string.Empty;
    private readonly Dictionary<DirectoryActivityType, IDisposable?> _subscriptions = new();

    #endregion

    #region Public fields and properties

    public event EventHandler? SettingsChanged;
    public bool EnableRaisingEvents
    {
        get => _directoryMonitor.EnableRaisingEvents;
        set
        {
            _directoryMonitor.EnableRaisingEvents = value;
            this.RaisePropertyChanged();
            NotifySettingsChanged();
        }
    }
    public string Filter
    {
        get => _directoryMonitor.Filter;
        set
        {
            this.RaiseAndSetIfChanged(ref _filter, value);
            _directoryMonitor.Filter = value;
            NotifySettingsChanged();
        }
    }
    public bool IncludeSubdirectories
    {
        get => _directoryMonitor.IncludeSubdirectories;
        set
        {
            if (_directoryMonitor.IncludeSubdirectories == value) return;
            
            _directoryMonitor.IncludeSubdirectories = value;
            this.RaisePropertyChanged();
            NotifySettingsChanged();
        }
    }
    public bool MonitoringChanged
    {
        get => _monitoringChanged;
        set
        {
            _monitoringChanged = value;
            UpdateSubscription(DirectoryActivityType.Changed, value, OnChanged);
            this.RaisePropertyChanged();
            NotifySettingsChanged();
        }
    }
    public bool MonitoringCreated
    {
        get => _monitoringCreated;
        set
        {
            _monitoringCreated = value;
            UpdateSubscription(DirectoryActivityType.Created, value, OnCreated);
            this.RaisePropertyChanged();
            NotifySettingsChanged();
        }
    }
    public bool MonitoringDeleted
    {
        get => _monitoringDeleted;
        set
        {
            _monitoringDeleted = value;
            UpdateSubscription(DirectoryActivityType.Deleted, value, OnDeleted);
            this.RaisePropertyChanged();
            NotifySettingsChanged();
        }
    }
    public bool MonitoringRenamed
    {
        get => _monitoringRenamed;
        set
        {
            _monitoringRenamed = value;
            UpdateSubscription(DirectoryActivityType.Renamed, value, OnRenamed);
            this.RaisePropertyChanged();
            NotifySettingsChanged();
        }
    }
    public string MostRecentLog
    {
        get => _mostRecentLog;
        set => this.RaiseAndSetIfChanged(ref _mostRecentLog, value);
    }
    public string Path { get; }
    
    #endregion
    
    public DirectoryMonitorViewModel(string path)
    {
        Path = path;
        _directoryMonitor = new DirectoryMonitor(path)
        {
            NotifyFilter = NotifyFilters.Attributes |
                           NotifyFilters.DirectoryName |
                           NotifyFilters.FileName |
                           NotifyFilters.LastWrite |
                           NotifyFilters.Security |
                           NotifyFilters.Size
        };
        EnableRaisingEvents = true;
        Filter = "*";
        MonitoringChanged = false;
        MonitoringCreated = true;
        MonitoringDeleted = true;
        MonitoringRenamed = true;
    }

    #region Private functions and methods
    
    private IDisposable CreateSubscription(
        DirectoryActivityType activityType,
        Action<DirectoryActivity> onActivity)
    {
        return _directoryMonitor.FileSystemChanges
            .Where(activity => activity.ActivityType == activityType)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(onActivity);
    }
    
    private void DisposeSubscription(DirectoryActivityType activityType)
    {
        if (!_subscriptions.TryGetValue(activityType, out var subscription)) return;
        
        subscription?.Dispose();
        _subscriptions.Remove(activityType);
    }

    private void NotifySettingsChanged()
    {
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }
    
    private void OnChanged(DirectoryActivity activity)
    {
        MostRecentLog = $"{activity.Name} Changed";
    }

    private void OnCreated(DirectoryActivity activity)
    {
        MostRecentLog = $"{activity.Name} Created";
    }

    private void OnDeleted(DirectoryActivity activity)
    {
        MostRecentLog = $"{activity.Name} Deleted";
    }
    
    private void OnRenamed(DirectoryActivity activity)
    {
        MostRecentLog = $"{activity.OldName} renamed to {activity.Name}";
    }
    
    private void UpdateSubscription(
        DirectoryActivityType activityType,
        bool shouldSubscribe,
        Action<DirectoryActivity> onActivity)
    {
        DisposeSubscription(activityType);

        if (!shouldSubscribe) return;
        
        _subscriptions[activityType] = CreateSubscription(activityType, onActivity);
    }
    
    #endregion
    
    #region Public functions and methods
    
    public void Dispose()
    {
        foreach (var subscription in _subscriptions.Values)
        {
            subscription?.Dispose();
        }
        _subscriptions.Clear();
        _directoryMonitor.Dispose();
        GC.SuppressFinalize(this);
    }

    #endregion
}