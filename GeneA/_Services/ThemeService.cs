using Avalonia.Styling;
using ModelA.Core;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GeneA._Helper.AppStateEnums;

namespace GeneA._Services;

public class ThemeService
{

    public ThemeService(IRepository<Settings> settingsRepo)
    {
        _settingsRepo = settingsRepo;
    }

    private readonly IRepository<Settings> _settingsRepo;

    public void ChangeTheme(AppTheme appTheme)
    {
        var settings = _settingsRepo.FindAll().FirstOrDefault();

        if (settings == null)
            return;

        switch (appTheme)
        {
            case AppTheme.Default:
                Avalonia.Application.Current!.RequestedThemeVariant = ThemeVariant.Light;
                settings.ColorTheme = 0;
                break;
            case AppTheme.Dark:
                Avalonia.Application.Current!.RequestedThemeVariant = ThemeVariant.Dark;
                settings.ColorTheme = 1;
                break;
            default:
                break;
        }

        _settingsRepo.Upsert(settings);
    }

    public void ChangeTheme(int appTheme)
    {
        var settings = _settingsRepo.FindAll().FirstOrDefault();

        if (settings == null)
            return;

        switch (appTheme)
        {
            case 0:
                Avalonia.Application.Current!.RequestedThemeVariant = ThemeVariant.Default;
                settings.ColorTheme = 0;
                break;
            case 1:
                Avalonia.Application.Current!.RequestedThemeVariant = ThemeVariant.Dark;
                settings.ColorTheme = 1;
                break;
            default:
                break;
        }

        _settingsRepo.Upsert(settings);
    }
}
