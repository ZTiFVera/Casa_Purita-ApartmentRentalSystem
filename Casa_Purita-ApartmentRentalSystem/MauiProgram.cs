using Casa_Purita_ApartmentRentalSystem.Services;
using Microsoft.Extensions.Logging;

namespace Casa_Purita_ApartmentRentalSystem
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register HttpClient + TenantService
            builder.Services.AddSingleton<HttpClient>();
            builder.Services.AddSingleton<TenantService>();

            #if DEBUG
            builder.Logging.AddDebug();
            #endif

            return builder.Build();
        }
    }

}

