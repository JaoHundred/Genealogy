using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using GeneA._Services;
using GeneA.ViewModels;
using GeneA.Views;
using Microsoft.Extensions.DependencyInjection;

namespace GeneA;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

         var mainViewModel = ConfigureServices().GetService<MainViewModel>();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = mainViewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<NavigationService>();

        ViewsViewModels(services);

        return services.BuildServiceProvider();
    }

    private static void ViewsViewModels(ServiceCollection services)
    {
        // Add the ViewModels as a service (Main as singleton, others as transient)

        services.AddSingleton<MainViewModel>();

        services.AddTransient<AddPersonView>();
        services.AddTransient<AddPersonViewModel>();
    }
}
