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

public class DirectoryMonitorViewModel : ViewModelBase
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

    public bool EnableRaisingEvents
    {
        get => _directoryMonitor.EnableRaisingEvents;
        set
        {
            _directoryMonitor.EnableRaisingEvents = value;
            this.RaisePropertyChanged();
        }
    }
    public string Filter
    {
        get => _directoryMonitor.Filter;
        set
        {
            this.RaiseAndSetIfChanged(ref _filter, value);
            _directoryMonitor.Filter = value;
        }
    }
    public bool IncludeSubdirectories
    {
        get => _directoryMonitor.IncludeSubdirectories;
        set
        {
            var wasRunning = _directoryMonitor.EnableRaisingEvents;
            
            if (wasRunning) _directoryMonitor.EnableRaisingEvents = false;

            _directoryMonitor.IncludeSubdirectories = value;

            if (wasRunning) _directoryMonitor.EnableRaisingEvents = true;

            this.RaisePropertyChanged();
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
        // Dispose of the current subscription if there is one
        if (_subscriptions.TryGetValue(activityType, out var subscription))
        {
            subscription?.Dispose();

            // Unsubscribe
            if (!shouldSubscribe)
            {
                _subscriptions.Remove(activityType);
                
                return;
            }
        }

        // Subscribe
        _subscriptions[activityType] = _directoryMonitor.FileSystemChanges
            .Where(activity => activity.ActivityType == activityType)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(onActivity);
    }
    
    #endregion
}