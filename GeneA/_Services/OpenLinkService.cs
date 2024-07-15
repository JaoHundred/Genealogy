using Avalonia.Controls;
using GeneA.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace GeneA._Services;

public class OpenLinkService
{
    public OpenLinkService(MainView mainView)
    {
        _mainView = mainView;
        _topLevel = TopLevel.GetTopLevel(mainView)!;
    }

    private MainView _mainView;
    private TopLevel _topLevel;

    //TODO: wait to next avalonia version to just call toplevel.launcher
    public async Task OpenLinkAsync(string link)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            link = link.Replace("&", "^&");
            Process.Start(new ProcessStartInfo(link) { UseShellExecute = true });
        }
        else //android
        {
            await Xamarin.Essentials.Launcher.OpenAsync(link);
        }
    }
}
