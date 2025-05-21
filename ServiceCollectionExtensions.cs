using CoffeeTime.Interfaces;
using CoffeeTime.Modules.DirectoryMonitor.Interfaces;
using CoffeeTime.Modules.DirectoryMonitor.Services;
using CoffeeTime.Services;
using CoffeeTime.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CoffeeTime;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection collection)
    {
        #region Services

        // Singletons
        collection.AddSingleton<ISaveDataService, SaveDataService>();
        
        // Transients
        collection.AddTransient<IDirectoryMonitor, DirectoryMonitor>();
        collection.AddTransient<INavigation, Navigation>();
        
        #endregion
        
        #region View models
        
        collection.AddTransient<MainWindowViewModel>();
        
        #endregion
    }
}