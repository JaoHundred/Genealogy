﻿using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Notification;
using GeneA._Helper;
using GeneA._Services;
using GeneA.Services;
using GeneA.ViewModels;
using GeneA.Views;
using Microsoft.Extensions.DependencyInjection;
using Model.Database;
using Model.Interfaces;
using Model.Services;
using ModelA.Database;

namespace GeneA;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public static ServiceProvider? ServiceProvider { get; private set; }

    public override void RegisterServices()
    {
        base.RegisterServices();

        ServiceProvider = ConfigureServices();

    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        var mainViewModel = ServiceProvider!.GetService<MainViewModel>();
        var mainView = ServiceProvider!.GetService<MainView>();


        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };

            desktop.MainWindow.Content = mainView;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            mainView!.DataContext = mainViewModel;
            singleViewPlatform.MainView = mainView;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<NavigationService>();
        services.AddSingleton<LiteDBConfiguration>();

        services.AddTransient<IGetFolderService, GetFolderService>();
        services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
        services.AddTransient<DocumentRepository>();
        services.AddTransient<INotificationMessageManager, NotificationMessageManager>();
        services.AddTransient<ThemeService>();
        services.AddTransient<FileService>();
        services.AddTransient<GraphService>();
        services.AddTransient<ImportExportService>();
        services.AddTransient<OpenLinkService>();
        


        ViewsViewModels(services);

        return services.BuildServiceProvider();
    }

    private static void ViewsViewModels(ServiceCollection services)
    {
        // Add the ViewModels as a service (Main as singleton, others as transient)

        services.AddSingleton<MainView>();
        services.AddSingleton<MainViewModel>();

        services.AddSingleton<HomeView>();
        services.AddSingleton<HomeViewModel>();

        services.AddSingleton<PersonListingView>();
        services.AddSingleton<PersonListingViewModel>();

        services.AddSingleton<SettingsView>();
        services.AddSingleton<SettingsViewModel>();

        services.AddTransient<PersonView>();
        services.AddTransient<PersonViewModel>();

        services.AddTransient<ConfirmationPopupView>();
        services.AddTransient<ConfirmationPopupViewModel>();

        services.AddTransient<OffspringsSelectionPopupView>();
        services.AddTransient<OffspringsSelectionPopupViewModel>();

        services.AddTransient<SpouseSelectionPopupView>();
        services.AddTransient<SpouseSelectionPopupViewModel>();

        services.AddTransient<NationalitySelectionPopupView>();
        services.AddTransient<NationalitySelectionPopupViewModel>();

        services.AddTransient<FamilyTreeView>();
        services.AddTransient<FamilyTreeViewModel>();
    }
}
