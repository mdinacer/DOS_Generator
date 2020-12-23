using System;
using System.IO;
using System.Windows;
using DOS_Generator.Core;
using DOS_Generator.Core.Models;
using DOS_Generator.Data;
using DOS_Generator.WPF.Domain;
using DOS_Generator.WPF.Services;
using DOS_Generator.WPF.ViewModels;
using DOS_Generator.WPF.ViewModels.Activities;
using DOS_Generator.WPF.ViewModels.Forms;
using DOS_Generator.WPF.ViewModels.Permanence;
using DOS_Generator.WPF.ViewModels.Settings;
using DOS_Generator.WPF.ViewModels.Template;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DOS_Generator.WPF
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public static User User { get; set; }
        public static AppSettings Settings { get; private set; }
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", false, true);
                }).ConfigureServices((context, services) => { ConfigureServices(context.Configuration, services); })
                .Build();

            ServiceProvider = _host.Services;
        }

        public static IServiceProvider ServiceProvider { get; private set; }
        public static Messenger Messenger { get; private set; }

        private static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            services.AddDbContext<SecurityDbContext>(
                options => options.UseSqlite(configuration.GetConnectionString("Default"),
                    x => x.MigrationsAssembly("Security.Data")));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<DeclarationTemplateViewModel>();
            services.AddSingleton<ActivitiesViewModel>();
            services.AddTransient<PermanenceViewModel>();
            services.AddTransient<AgencyFormViewModel>();
            services.AddTransient<PortsViewModel>();
            services.AddTransient<ShipsViewModel>();
            services.AddTransient<OfficersViewModel>();
            services.AddTransient<AgenciesViewModel>();
            services.AddTransient<DeclarationFormViewModel>();
            services.AddTransient<MainWindow>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            if (_host != null)
                await _host.StartAsync();

            Messenger = new Messenger();

            var window = ServiceProvider.GetRequiredService<MainWindow>();
            window.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            using (_host)
            {
                await _host.StopAsync(TimeSpan.FromSeconds(5));
            }

            base.OnExit(e);
        }
    }
}