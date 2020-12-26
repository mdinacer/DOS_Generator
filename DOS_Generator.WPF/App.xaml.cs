using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using DOS_Generator.Core;
using DOS_Generator.Core.Models;
using DOS_Generator.Data;
using DOS_Generator.WPF.Domain;
using DOS_Generator.WPF.Services;
using DOS_Generator.WPF.ViewModels.Activities;
using DOS_Generator.WPF.ViewModels.Forms;
using DOS_Generator.WPF.ViewModels.Permanence;
using DOS_Generator.WPF.ViewModels.Settings;
using DOS_Generator.WPF.ViewModels.Template;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Win32;

namespace DOS_Generator.WPF
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private readonly IHost _host;

        public App()
        {
            try
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
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public static User User { get; set; }
        public static AppSettings Settings { get; set; }
        public static Messenger Messenger { get; private set; }

        public static IServiceProvider ServiceProvider { get; private set; }

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
            services.AddTransient<UserViewModel>();
            services.AddTransient<AgenciesViewModel>();
            services.AddTransient<DeclarationFormViewModel>();
            services.AddTransient<GeneralSettingsViewModel>();

            services.AddTransient<MainWindow>();
        }

        private static bool CheckMsWord()
        {
            using var regWord = Registry.ClassesRoot.OpenSubKey("Word.Application");

            return regWord != null;
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            var isWordInstalled = CheckMsWord();

            if (!isWordInstalled)
            {
                MessageBox.Show("Microsoft Word is not installed, please install it from Microsoft market");
                Current.Shutdown();
            }
            if (_host != null)
                await _host.StartAsync();
            Messenger = new Messenger();

            if (!File.Exists(@".\AppSettings.xml"))
                AppSettingsServices.WriteSettings(new AppSettings());

            Settings = AppSettingsServices.ReadSettings();

            EventManager.RegisterClassHandler(typeof(TextBox),
                UIElement.GotFocusEvent,
                new RoutedEventHandler(TextBox_GotFocus));

            EventManager.RegisterClassHandler(typeof(ComboBox),
                UIElement.GotFocusEvent,
                new RoutedEventHandler(TextBox_GotFocus));

            var window = ServiceProvider.GetRequiredService<MainWindow>();
            window.Show();

            base.OnStartup(e);
        }

        private static void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            switch (sender)
            {
                case TextBox textBox:
                    textBox.SelectAll();
                    break;

                case ComboBox comboBox when comboBox.IsEditable:
                    var cbTextBox = comboBox.Template.FindName("PART_EditableTextBox", comboBox) as TextBox;
                    cbTextBox?.SelectAll();
                    break;
            }
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