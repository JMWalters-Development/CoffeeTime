using System.Collections.ObjectModel;
using CoffeeTime.Modules.DirectoryMonitors.Interfaces;
using CoffeeTime.ViewModels;

namespace CoffeeTime.Modules.DirectoryMonitors.ViewModels;

public class DirectoryMonitorsViewModel : ViewModelBase
{
    private readonly IDirectoryMonitorManager _directoryMonitorManager;
    
    public ObservableCollection<DirectoryMonitorViewModel> DirectoryMonitorVms => _directoryMonitorManager.DirectoryMonitorVms;
    
    public DirectoryMonitorsViewModel(IDirectoryMonitorManager directoryMonitorManager)
    {
        _directoryMonitorManager = directoryMonitorManager;
    }
}