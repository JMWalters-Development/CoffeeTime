using System.Reactive;
using Avalonia.Controls.ApplicationLifetimes;
using CoffeeTime.Interfaces;
using CoffeeTime.Modules.DirectoryMonitors.ViewModels;
using ReactiveUI;

namespace CoffeeTime.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    #region Private fields and properties
    
    private readonly ObservableAsPropertyHelper<ReactiveObject?> _currentModuleVm;
    
    #endregion
    
    #region Public fields and properties
    
    public ReactiveObject? CurrentModuleVm => _currentModuleVm.Value;
    public ReactiveCommand<Unit, Unit> ExitCommand { get; }
    
    #endregion

    public MainWindowViewModel(INavigation navigation, ISaveDataService saveData)
    {
        ExitCommand = ReactiveCommand.Create(OnExit);
        navigation
            .CurrentVmChanged
            .ToProperty(this, vm => vm.CurrentModuleVm, out _currentModuleVm);
        var temporaryDmVm = saveData.LoadJson<DirectoryMonitorViewModel>("temporaryDmVm.json") ??
                            new DirectoryMonitorViewModel(@"C:\Users");
        
        temporaryDmVm.SettingsChanged += (sender, args) => saveData.SaveJson(temporaryDmVm, "temporaryDmVm.json");
        navigation.NavigateTo(temporaryDmVm);
    }
    
    #region Private functions and methods
    
    private static void OnExit()
    {
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }

    #endregion
}