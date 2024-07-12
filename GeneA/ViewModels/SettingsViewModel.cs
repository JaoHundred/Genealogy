using CommunityToolkit.Mvvm.Input;
using GeneA._Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GeneA._Helper.AppStateEnums;
using MvvmHelpers;
using CommunityToolkit.Mvvm.ComponentModel;
using GeneA.ViewModelItems;
using Model.Interfaces;
using Avalonia.Threading;
using Avalonia.Controls;
using Avalonia.Styling;
using System.Reactive.Linq;
using GeneA._Services;
using ModelA.Core;
using GeneA.Services;
using Avalonia.Notification;
using System.Text.Json;
using Avalonia.Platform;
using Avalonia;
using System.IO;

namespace GeneA.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    public SettingsViewModel(MainViewModel mainViewModel, IRepository<Settings> settingsRepository, ThemeService themeService,
        ImportExportService importExportService)
    {
        _mainViewModel = mainViewModel;
        _mainViewModel.Title = DynamicTranslate.Translate(MessageConsts.Settings);

        _settingsRepository = settingsRepository;
        _themeService = themeService;
        _importExportService = importExportService;

        AppThemes = new List<AppThemeItemViewModel>
        {
            new AppThemeItemViewModel()
            {
                AppTheme = AppTheme.Default,
                Name = DynamicTranslate.Translate(MessageConsts.Light),
            },
            new AppThemeItemViewModel()
            {
                AppTheme = AppTheme.Dark,
                Name = DynamicTranslate.Translate(MessageConsts.Dark),
            },
        };

        LicenseItems = new List<LicenseItemViewModel>();

        LoadAction = () => { Load().SafeFireAndForget(); };

    }

    private readonly IRepository<Settings> _settingsRepository;
    private readonly ThemeService _themeService;
    private readonly ImportExportService _importExportService;
    private readonly MainViewModel _mainViewModel;

    [ObservableProperty]
    private List<LicenseItemViewModel> _licenseItems;

    public List<AppThemeItemViewModel> AppThemes { get; set; }

    [ObservableProperty]
    private AppThemeItemViewModel? _selectedAppTheme;
    partial void OnSelectedAppThemeChanged(AppThemeItemViewModel? value)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            if (value != null)
            {
                _themeService.ChangeTheme(value.AppTheme);
            }
        });
    }

    private Settings? _settings;
    private async Task Load()
    {
        await Task.Run(() =>
        {
            _settings = _settingsRepository.FindAll().FirstOrDefault();

            if (_settings == null)
                return;

            AppTheme theme = AppTheme.Default;

            switch (_settings.ColorTheme)
            {
                case 0:
                    theme = AppTheme.Default;
                    break;
                case 1:
                    theme = AppTheme.Dark;
                    break;
            }

            SelectedAppTheme = AppThemes.FirstOrDefault(p => p.AppTheme == theme);

            Stream str = AssetsHelper.Open("Licenses.json");

            var licenses = JsonSerializer.Deserialize<IEnumerable<LicenseItemViewModel>>(str);
            if (licenses != null)
                LicenseItems.AddRange(licenses);
        });
    }


    [RelayCommand]
    private void OpenLicense(string link)
    {
        //TODO:open license link in default browser
    }

    [RelayCommand]
    private async Task Import()
    {
        bool importedSuccess = await _importExportService.Import();

        if (importedSuccess)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                _mainViewModel.NotificationManager.CreateMessage()
                .Animates(true)
                .HasMessage(DynamicTranslate.Translate(MessageConsts.ImportedWithSuccess))
                .Dismiss().WithDelay(TimeSpan.FromSeconds(2))
                .Queue();
            });
        }
    }

    [RelayCommand]
    private async Task Export()
    {
        bool exportedSuccess = await _importExportService.Export();

        if (exportedSuccess)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                _mainViewModel.NotificationManager.CreateMessage()
                .Animates(true)
                .HasMessage(DynamicTranslate.Translate(MessageConsts.ExportedWithSuccess))
                .Dismiss().WithDelay(TimeSpan.FromSeconds(2))
                .Queue();
            });
        }
    }
}
