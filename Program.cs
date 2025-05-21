using Avalonia;
using Avalonia.ReactiveUI;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace CoffeeTime;

sealed class Program
{
    #region Public fields and properties
    
    public static IServiceProvider ServiceProvider { get; private set; } = null!;
    
    #endregion
    
    #region Private functions and methods

    private static void ConfigureCoreServices()
    {
        
    }
    
    #endregion
    
    #region Public functions and methods
    
    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
    
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddCommonServices();
        ServiceProvider = services.BuildServiceProvider();
        ConfigureCoreServices();
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }
    
    #endregion
}