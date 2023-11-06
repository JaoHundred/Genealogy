using Android.App;
using Android.Content.PM;

using Avalonia;
using Avalonia.Android;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.MaterialDesign;

namespace GeneA.Android;

[Activity(
    Label = "GeneA.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        IconProvider.Current.Register<MaterialDesignIconProvider>();

        return base.CustomizeAppBuilder(builder)
            .WithInterFont();
    }
}
