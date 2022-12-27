using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OceanBattle.Client.ViewModels;
using OceanBattle.Client.Views;
using AspNetCoreInjection.TypedFactories;
using OceanBattle.Client.Factories;

namespace OceanBattle.Client
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override async void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                IHost appHost = Host.CreateDefaultBuilder()
                                    .ConfigureServices(ConfigureServices)
                                    .Build();

                await appHost.StartAsync();

                MainWindowViewModel viewModel = appHost.Services.GetRequiredService<MainWindowViewModel>();

                desktop.MainWindow = new MainWindow
                {
                    DataContext = viewModel,
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            services.AddSingleton<MainWindowViewModel>();

            services.RegisterTypedFactory<ILogInViewModelFactory>()
                    .ForConcreteType<LogInViewModel>();

            services.RegisterTypedFactory<IRegisterViewModelFactory>()
                    .ForConcreteType<RegisterViewModel>();

            services.AddTransient<ViewChanger>(provider => (viewModel) =>
            {
                MainWindowViewModel mainViewModel = provider.GetRequiredService<MainWindowViewModel>();
                mainViewModel.Content = viewModel;
            });

            services.AddTransient<GameHubViewModel>();
            services.AddTransient<InvitesViewModel>();
            services.AddTransient<ActivePlayersViewModel>();
        }
    }
}