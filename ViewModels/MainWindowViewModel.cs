using CoffeeTime.Interfaces;
using ReactiveUI;

namespace CoffeeTime.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    #region Private fields and properties
    
    private readonly ObservableAsPropertyHelper<ReactiveObject?> _currentModuleVm;
    
    #endregion
    
    #region Public fields and properties
    
    public ReactiveObject? CurrentModuleVm => _currentModuleVm.Value;
    
    #endregion

    public MainWindowViewModel(INavigationService navigationService)
    {
        navigationService
            .CurrentVmChanged
            .ToProperty(this, ns => ns.CurrentModuleVm, out _currentModuleVm);
    }
}