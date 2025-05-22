using System;
using System.Collections.ObjectModel;
using CoffeeTime.Interfaces;
using CoffeeTime.Modules.DirectoryMonitors.Interfaces;
using CoffeeTime.Modules.DirectoryMonitors.ViewModels;

namespace CoffeeTime.Modules.DirectoryMonitors.Services;

public class DirectoryMonitorManager : IDirectoryMonitorManager, IDisposable
{
    public ObservableCollection<DirectoryMonitorViewModel> DirectoryMonitorVms { get; }

    public DirectoryMonitorManager(ISaveDataService saveData)
    {
        const string saveDataPath = @"DirectoryMonitors\DirectoryMonitorVms.json";
        var loadedData = saveData.LoadJson<ObservableCollection<DirectoryMonitorViewModel>>(saveDataPath);
        
        if (loadedData == null)
        {
            loadedData = [
                new DirectoryMonitorViewModel(@"C:\"),
                new DirectoryMonitorViewModel(@"C:\Users")
            ];
            saveData.SaveJson(loadedData, saveDataPath);
        }
        
        DirectoryMonitorVms = loadedData;

        foreach (var directoryMonitorVm in DirectoryMonitorVms)
        {
            directoryMonitorVm.SettingsChanged += (_, _) => saveData.SaveJson(loadedData, saveDataPath);
        }
    }
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}