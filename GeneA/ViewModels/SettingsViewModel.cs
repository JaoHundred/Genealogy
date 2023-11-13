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

namespace GeneA.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    public SettingsViewModel(MainViewModel mainViewModel)
    {
        mainViewModel.Title = DynamicTranslate.Translate(MessageConsts.Settings);

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

    public List<AppThemeItemViewModel> AppThemes { get; set; }

    [ObservableProperty]
    private AppThemeItemViewModel? _selectedAppTheme;

    private async Task Load()
    {
        await Task.Run(() => { });
    }


    [RelayCommand]
    private void ChangeAppTheme()
    {

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
