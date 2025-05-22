using System.Collections.Generic;
using System.Collections.ObjectModel;
using CoffeeTime.Modules.DirectoryMonitors.Interfaces;
using CoffeeTime.ViewModels;

namespace CoffeeTime.Modules.DirectoryMonitors.ViewModels;

public class DirectoryMonitorsViewModel(IDirectoryMonitorManager directoryMonitorManager) : ViewModelBase
{
    public IReadOnlyCollection<DirectoryMonitorViewModel> DirectoryMonitorVms =>
        new ReadOnlyObservableCollection<DirectoryMonitorViewModel>(directoryMonitorManager.DirectoryMonitorVms);
}