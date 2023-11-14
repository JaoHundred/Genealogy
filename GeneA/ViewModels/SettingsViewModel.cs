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
using ModelA.Core;
using Avalonia.Threading;
using Avalonia.Controls;
using Avalonia.Styling;
using System.Reactive.Linq;
using GeneA._Services;

namespace GeneA.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    public SettingsViewModel(MainViewModel mainViewModel, IRepository<Settings> settingsRepository, ThemeService themeService)
    {
        mainViewModel.Title = DynamicTranslate.Translate(MessageConsts.Settings);

        _settingsRepository = settingsRepository;
        _themeService = themeService;

        AppThemes = new List<AppThemeItemViewModel>
        {
            new AppThemeItemViewModel()
            {
                AppTheme = AppTheme.Light,
                Name = DynamicTranslate.Translate(MessageConsts.Light),
            },
            new AppThemeItemViewModel()
            {
                AppTheme = AppTheme.Dark,
                Name = DynamicTranslate.Translate(MessageConsts.Dark),
            },
        };

        LoadAction = () => { Load().SafeFireAndForget(); };

    }

    private readonly IRepository<Settings> _settingsRepository;
    private readonly ThemeService _themeService;

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
            _settings = _settingsRepository.FindById(1);

            AppTheme theme = AppTheme.Light;

            switch (_settings.ColorTheme)
            {
                case 0:
                    theme = AppTheme.Light;
                    break;
                case 1:
                    theme = AppTheme.Dark;
                    break;
            }

            SelectedAppTheme = AppThemes.FirstOrDefault(p => p.AppTheme == theme);
        });
    }


    [RelayCommand]
    private void OpenLicenses()
    {
        //TODO:open popup with all the used libs licenses if you click in one item it will open the browser with its respective
        //page
    }

    [RelayCommand]
    private void Import()
    {

    }

    [RelayCommand]
    private void Export()
    {

    }
}
