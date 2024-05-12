using BTGOpcoes.ViewModels;
using BTGOpcoes.Views;
using Microsoft.Extensions.Logging;
namespace BTGOpcoes
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

            builder.Services.AddViewModel<OptionsCalcViewModel, OptionsCalcPage>();

            return builder.Build();
        }
        private static void AddViewModel<TViewModel, TView>(this IServiceCollection services)
            where TView : ContentPage, new()
            where TViewModel : class
        {
            services.AddTransient<TViewModel>();
            services.AddTransient<TView>(s => new TView() { BindingContext = s.GetRequiredService<TViewModel>() });
        }
    }
}
