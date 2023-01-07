using AspNetCoreInjection.TypedFactories;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using OceanBattle.Client.Core.Abstractions;
using OceanBattle.Client.DataStore;
using OceanBattle.Client.Factories;
using OceanBattle.Client.ViewModels;
using OceanBattle.Client.Views;
using OcenBattle.Client.Core.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using OceanBattle.DataModel.ClientData;

namespace OceanBattle.Client
{
    public partial class App : Application
    {
        private const string url = "https://localhost:49153/";

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
            services.AddTransient<ActivePlayersViewModel>();

            services.RegisterTypedFactory<ICreatorViewModelFactory>()
                .ForConcreteType<CreatorViewModel>();

            services.RegisterTypedFactory<IGameHubViewModelFactory>()
                .ForConcreteType<GameHubViewModel>();

            services.RegisterTypedFactory<IGameViewModelFactory>()
                .ForConcreteType<GameViewModel>();

            services.RegisterTypedFactory<IDeploymentViewModelFactory>()
                .ForConcreteType<DeploymentViewModel>();

            services.RegisterTypedFactory<IBattleViewModelFactory>()
                .ForConcreteType<BattleViewModel>();

            services.RegisterTypedFactory<IEndViewModelFactory>()
                .ForConcreteType<EndViewModel>();

            HttpClient httpClient = new HttpClient
            {
                BaseAddress = new Uri(url + "api/")
            };

            services.AddSingleton(httpClient);

            HubConnection connection = new HubConnectionBuilder()
                .WithUrl(url + "gameHub", options =>
                {
                    options.AccessTokenProvider = 
                    () => Task.FromResult(httpClient.DefaultRequestHeaders!.Authorization!.Parameter);
                })
                .WithAutomaticReconnect()
                .AddNewtonsoftJsonProtocol(options =>
                {
                    options.PayloadSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.PayloadSerializerSettings.Formatting = Formatting.None;
                    options.PayloadSerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
                    options.PayloadSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
                    options.PayloadSerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    options.PayloadSerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
                    options.PayloadSerializerSettings.ContractResolver = new DefaultContractResolver();
                })
                .Build();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.ShutdownRequested += 
                    async (source, args) => await connection.InvokeAsync("MakeInactive");
            }

            services.AddSingleton(connection);

            services.AddTransient<IAuthApiClient, AuthApiClient>();
            services.AddTransient<IUserApiClient, UserApiClient>();
            services.AddTransient<IClientDataStore, ClientDataStore>();
            services.AddDataProtection();
            services.AddDbContext<ClientDataStoreContext>(
                options => options.UseSqlite("FileName=./ClientDataStore.db"));

            services.AddTransient<IGameApiClient, GameApiClient>();
        }
    }
}