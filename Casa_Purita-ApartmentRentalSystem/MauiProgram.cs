using Casa_Purita_ApartmentRentalSystem.MVVM.Views;
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

            // Services
            builder.Services.AddSingleton<HttpClient>();
            builder.Services.AddSingleton<TenantService>();

            // Views
            builder.Services.AddTransient<HomeView>();
            builder.Services.AddTransient<MainView>();
            builder.Services.AddTransient<CreateView>();
            builder.Services.AddTransient<UpdateView>();
            builder.Services.AddTransient<DeletedTenantsView>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}