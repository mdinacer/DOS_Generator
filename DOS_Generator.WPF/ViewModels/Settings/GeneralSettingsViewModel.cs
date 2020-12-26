using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using DOS_Generator.Core;
using DOS_Generator.Core.Models;
using DOS_Generator.WPF.Services;
using DOS_Generator.WPF.Views.Dialogs;
using DOS_Generator.WPF.Views.Forms;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;

namespace DOS_Generator.WPF.ViewModels.Settings
{
    public class GeneralSettingsViewModel : ViewModelBase
    {
        #region Properties

        public string TemplatePath { get; set; }
        public string MessagePath { get; set; }

        public ObservableCollection<MailServer> Servers { get; set; }

        private readonly IUnitOfWork _unitOfWork;
        private ICollectionView _servers;

        #endregion

        #region Constructors

        public GeneralSettingsViewModel()
        {
            
        }

        public GeneralSettingsViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            LoadData();
        }

        #endregion

        #region Functions

        private async void LoadData()
        {
            var settings = App.Settings;

            TemplatePath = Path.GetFullPath(settings.TemplatePath);
            MessagePath = Path.GetFullPath(settings.MessagePath);

            await LoadServers();
        }

        private async Task LoadServers()
        {
            var servers = (await _unitOfWork.MailServers.GetAllAsync())
                .OrderBy(server => server.ServiceName)
                .ToList();

            Servers = servers.Any()
                ? new ObservableCollection<MailServer>(servers)
                : new ObservableCollection<MailServer>();

            _servers = CollectionViewSource.GetDefaultView(Servers);
            _servers.SortDescriptions.Add(new SortDescription("ServiceName",ListSortDirection.Ascending));
            _servers.Filter = FilterServers;
        }

        private bool FilterServers(object obj)
        {
            if (!(obj is MailServer server)) return false;
            return !server.IsDeleted;
        }

        private static void OpenFile(string path)
        {
            if(!File.Exists(path)) return;
            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true,
                Verb = "open"
            });
        }

        private bool IsCustomServer(MailServer server)
        {
            return server != null 
                   && !server.ServiceName.Contains("google", StringComparison.InvariantCultureIgnoreCase)
                   && !server.ServiceName.Contains("outlook", StringComparison.InvariantCultureIgnoreCase);
        }

        private void ViewFile(string value)
        {
            if(string.IsNullOrEmpty(value)) return;

            switch (value)
            {
                case "Template":
                    OpenFile(TemplatePath);
                    break;
                case "Message":
                    OpenFile(MessagePath);
                    break;
            }
        }

        private async void AddServer()
        {
            var server = new MailServer();
            var result = await ShowServerDialog(server);

            if(!result) return;

            if(string.IsNullOrEmpty(server.ServiceName) 
               || string.IsNullOrEmpty(server.Host) 
               || server.Port == 0) return;

            await _unitOfWork.MailServers.AddAsync(server);
            await _unitOfWork.CommitAsync();
            Servers.Add(server);
        }

        private async void EditServer(MailServer server)
        {
            var result = await ShowServerDialog(server);

            if (!result) return;
            await _unitOfWork.CommitAsync();
        }

        private async void RemoveServer(MailServer server)
        {
            var view = new ConfirmationDialog($"This server {server.ServiceName} will be permanently deleted, Proceed?", "Delete email server");

            var result = (bool)await DialogHost.Show(view);

            if (!result) return;
            server.IsDeleted = true;
            await _unitOfWork.CommitAsync();
        }

        private async Task<bool> ShowServerDialog(MailServer server)
        {
            var view = new EmailServerForm {DataContext = server};

            return (bool) await DialogHost.Show(view);
        }

        private void CheckMsWord()
        {
            using var regWord = Registry.ClassesRoot.OpenSubKey("Word.Application");
            System.Windows.MessageBox.Show(regWord == null
                ? "Microsoft Word is not installed"
                : "Microsoft Word is installed");
        }

        #endregion

        #region Events

        #endregion

        #region Commands

        public ICommand ViewCommand => new RelayCommand<string>(ViewFile);

        public ICommand AddServerCommand => new RelayCommand(o => AddServer());
        public ICommand EditServerCommand => new RelayCommand<MailServer>(EditServer);
        public ICommand RemoveServerCommand => new RelayCommand<MailServer>(RemoveServer, IsCustomServer);

        #endregion

    }
}