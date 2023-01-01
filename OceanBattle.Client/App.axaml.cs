using AspNetCoreInjection.TypedFactories;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OceanBattle.Client.Core.Abstractions;
using OceanBattle.Client.DataStore;
using OceanBattle.Client.Factories;
using OceanBattle.Client.ViewModels;
using OceanBattle.Client.Views;
using OcenBattle.Client.Core.Services;
using System;
using System.Net.Http;

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

            //services.AddTransient<GameHubViewModel>();
            services.AddTransient<InvitesViewModel>();
            services.AddTransient<ActivePlayersViewModel>();
            
            services.AddSingleton(new HttpClient
            {
                BaseAddress = new Uri("https://localhost:49155/api/")
            });

            services.AddTransient<IAuthApiClient, AuthApiClient>();
            services.AddTransient<IUserApiClient, UserApiClient>();
            services.AddTransient<IClientDataStore, ClientDataStore>();
            services.AddDataProtection();
            services.AddDbContext<ClientDataStoreContext>(
                options => options.UseSqlite("FileName=./ClientDataStore.db"));

            services.RegisterTypedFactory<IGameHubViewModelFactory>()
                .ForConcreteType<GameHubViewModel>();
        }
    }
}