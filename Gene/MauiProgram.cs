using CommunityToolkit.Maui;
using Gene.Modules.Home;
using Microsoft.Extensions.Logging;
using Model.Database;
using Model.Interfaces;
using Model.Services;

namespace Gene
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            InitializeServices(builder);
            InitializeModules(builder);

            return builder.Build();
        }

        private static void InitializeServices(MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<LiteDBConfiguration>();
            builder.Services.AddScoped<IGetFolderService, GetFolderService>();

            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        }

        private static void InitializeModules(MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<HomeView>();
            builder.Services.AddSingleton<HomeViewModel>();
        }
    }
}