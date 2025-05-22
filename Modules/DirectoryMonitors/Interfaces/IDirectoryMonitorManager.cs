using System.Collections.ObjectModel;
using CoffeeTime.Modules.DirectoryMonitors.ViewModels;

namespace CoffeeTime.Modules.DirectoryMonitors.Interfaces;

public interface IDirectoryMonitorManager
{
    ObservableCollection<DirectoryMonitorViewModel> DirectoryMonitorVms { get; }
}