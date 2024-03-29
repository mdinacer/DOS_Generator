﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DOS_Generator.Core;
using DOS_Generator.Core.Models;
using DOS_Generator.WPF.Domain;
using DOS_Generator.WPF.Helpers;
using DOS_Generator.WPF.Services;
using DOS_Generator.WPF.Views.Forms;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;

namespace DOS_Generator.WPF.ViewModels.Forms
{
    public class UserFormViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        #region Events

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(Email):
                    AutoSelectServer();
                    break;
            }
        }

        #endregion

        #region Commands

        public ICommand BrowseCommand => new RelayCommand(o => BrowseImage());
        public ICommand AddNewServerCommand => new RelayCommand(o => AddNewServer());

        #endregion

        #region Properties

        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string Email { get; set; }
        public string EmailPassword { get; set; }

        public ObservableCollection<MailServer> Servers { get; set; }
        public MailServer SelectedServer { get; set; }

        #region Officer

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Initials { get; set; }
        public string TemplatePath { get; set; }

        #endregion

        #region Validation

        public bool HasErrors => _validationErrorsByProperty.Any();
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private readonly Dictionary<string, List<object>> _validationErrorsByProperty =
            new Dictionary<string, List<object>>();

        #endregion

        public bool IsEdit { get; set; }

        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region Constructors

        public UserFormViewModel()
        {
        }

        public UserFormViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            LoadServers();
        }

        #endregion

        #region Functions

        private async void LoadServers()
        {
            var servers = (await _unitOfWork.MailServers.GetAllAsync())
                .OrderBy(server => server.ServiceName)
                .ToList();
            Servers = servers.Any()
                ? new ObservableCollection<MailServer>(servers)
                : new ObservableCollection<MailServer>();
            AddDefaultServices();
        }

        private void AddDefaultServices()
        {
            var list = new List<MailServer>
            {
                new MailServer {ServiceName = "GMail", Host = "smtp.gmail.com", Port = 587},
                new MailServer {ServiceName = "Outlook", Host = "smtp.office365.com", Port = 587},
                new MailServer {ServiceName = "Yahoo", Host = "smtp.mail.yahoo.com", Port = 587}
            };

            list.ForEach(async server =>
            {
                if (!Servers.Any(s => s.ServiceName.Equals(server.ServiceName)))
                {
                    Servers.Add(server);
                    await _unitOfWork.MailServers.AddAsync(server);
                }

                await _unitOfWork.CommitAsync();

            });
        }

        public void LoadUser(User user)
        {
            if (user == null) return;
            IsEdit = true;
            UserName = user.Name;
            Email = user.Email;
            EmailPassword = user.EmailPassword == null ? null : GetEmailPassword(user.EmailPassword);
            SelectedServer = user.MailServer;

            FirstName = user.Officer.FirstName;
            LastName = user.Officer.LastName;
            Title = user.Officer.Title;
            Address = user.Officer.Address;
            Phone = user.Officer.Phone;
            Initials = user.Officer.Initials;
            TemplatePath = user.Officer.TemplatePath;
        }

        private string GetEmailPassword(byte[] value)
        {
            var bytesPassword = DataProtectionService.Unprotect(value);
            return DataProtectionService.DecodeString(bytesPassword);
        }

        private byte[] SetEmailPassword()
        {
            if (string.IsNullOrEmpty(EmailPassword)) return null;
            var encodedPassword = DataProtectionService.EncodeString(EmailPassword);
            return DataProtectionService.Protect(encodedPassword);
        }

        public void UpdateUser(User user)
        {
            if (user == null) return;
            
            user.Email = Email;
            user.EmailPassword = SetEmailPassword();
            user.MailServer = SelectedServer;
            user.Officer.FirstName = FirstName;
            user.Officer.LastName = LastName;
            user.Officer.Title = Title;
            user.Officer.Address = Address;
            user.Officer.Phone = Phone;
            user.Officer.Initials = Initials;
            user.Officer.TemplatePath = SetTemplate();
        }

        private Officer CreateOfficer()
        {
            return new Officer
            {
                FirstName = FirstName,
                LastName = LastName,
                Title = Title,
                Address = Address,
                Phone = Phone,
                Initials = Initials,
                TemplatePath = SetTemplate()
            };
        }

        public async Task<User> CreateUser()
        {
            var user = AccountService.Register(new LoginModel
            {
                UserName = UserName,
                Password = UserPassword
            });

            user.Email = Email;
            user.EmailPassword = SetEmailPassword();
            user.MailServer = SelectedServer;

            user.Officer = CreateOfficer();

            await _unitOfWork.Users.AddAsync(user);

            await _unitOfWork.CommitAsync();

            return user;
        }

        private void BrowseImage()
        {
            var dialog = new OpenFileDialog();

            if (dialog.ShowDialog() != true) return;

            TemplatePath = dialog.FileName;
        }

        private string SetTemplate()
        {
            if (string.IsNullOrWhiteSpace(TemplatePath)) return null;
            if (!Directory.Exists(".\\Resources\\"))
                Directory.CreateDirectory(".\\Resources\\");

            var outputFile = $".\\Resources\\{Path.GetRandomFileName()}{Path.GetExtension(TemplatePath)}";
            EncryptionService.EncryptFile(UserName, TemplatePath, outputFile);
            return outputFile;
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (_validationErrorsByProperty.TryGetValue(propertyName, out var errors)) return errors;
            return Array.Empty<object>();
        }

        private async void Validate(string changedPropertyName)
        {
            switch (changedPropertyName)
            {
                case nameof(UserName) when !IsEdit:
                {
                    if (string.IsNullOrWhiteSpace(UserName))
                    {
                        _validationErrorsByProperty[nameof(UserName)] =
                            new List<object> {"This field can't be empty"};
                        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(UserName)));
                    }
                    else if ((await _unitOfWork.Users.GetAllAsync()).Any(user =>
                        user.Name.Equals(UserName, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        _validationErrorsByProperty[nameof(UserName)] =
                            new List<object> {"Another user with the same name already exists"};
                        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(UserName)));
                    }
                    else if (_validationErrorsByProperty.Remove(nameof(UserName)))
                    {
                        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(UserName)));
                    }

                    break;
                }

                case nameof(UserPassword):
                {
                    if (string.IsNullOrWhiteSpace(UserPassword))
                    {
                        _validationErrorsByProperty[nameof(UserPassword)] =
                            new List<object> {"This field can't be empty"};
                        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(UserPassword)));
                    }
                    else if (UserPassword.Length < 4)
                    {
                        _validationErrorsByProperty[nameof(UserPassword)] =
                            new List<object> {"The password must contain more than 4 characters"};
                        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(UserPassword)));
                    }
                    else if (_validationErrorsByProperty.Remove(nameof(UserPassword)))
                    {
                        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(UserPassword)));
                    }

                    break;
                }

                case nameof(FirstName):
                {
                    if (string.IsNullOrWhiteSpace(FirstName))
                    {
                        _validationErrorsByProperty[nameof(FirstName)] =
                            new List<object> {"This field can't be empty"};
                        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(FirstName)));
                    }
                    else if (_validationErrorsByProperty.Remove(nameof(FirstName)))
                    {
                        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(FirstName)));
                    }

                    break;
                }

                case nameof(LastName):
                {
                    if (string.IsNullOrWhiteSpace(LastName))
                    {
                        _validationErrorsByProperty[nameof(LastName)] =
                            new List<object> {"This field can't be empty"};
                        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(LastName)));
                    }
                    else if (_validationErrorsByProperty.Remove(nameof(LastName)))
                    {
                        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(LastName)));
                    }

                    break;
                }

                case nameof(Title):
                {
                    if (string.IsNullOrWhiteSpace(Title))
                    {
                        _validationErrorsByProperty[nameof(Title)] =
                            new List<object> {"This field can't be empty"};
                        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Title)));
                    }
                    else if (_validationErrorsByProperty.Remove(nameof(Title)))
                    {
                        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Title)));
                    }

                    break;
                }

                case nameof(Initials):
                {
                    if (string.IsNullOrWhiteSpace(Initials))
                    {
                        _validationErrorsByProperty[nameof(Initials)] =
                            new List<object> {"This field can't be empty"};
                        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Initials)));
                    }
                    else if (_validationErrorsByProperty.Remove(nameof(Initials)))
                    {
                        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Initials)));
                    }

                    break;
                }

                case nameof(Email):
                {
                    if (!string.IsNullOrWhiteSpace(Email) && !EmailAddressCheck.CheckEmail(Email))
                    {
                        _validationErrorsByProperty[nameof(Email)] =
                            new List<object> {"Not valid email address"};
                        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Email)));
                    }

                    break;
                }

                case nameof(SelectedServer):
                {
                    if (SelectedServer == null)
                    {
                        _validationErrorsByProperty[nameof(SelectedServer)] =
                            new List<object> {"Please select a mail server configuration"};
                        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(SelectedServer)));
                    }

                    break;
                }
            }
        }

        public bool ValidateFields()
        {
            Validate(nameof(UserName));
            Validate(nameof(UserPassword));
            Validate(nameof(Email));
            if (!string.IsNullOrWhiteSpace(Email))
                Validate(nameof(SelectedServer));
            Validate(nameof(FirstName));
            Validate(nameof(LastName));
            Validate(nameof(Title));
            Validate(nameof(Initials));

            return HasErrors;
        }

        private async void AddNewServer()
        {
            var server = new MailServer();

            var view = new EmailServerForm {DataContext = server};

            var isOk = (bool) await DialogHost.Show(view, "UserFormDialogHost");

            if (!isOk) return;

            if (string.IsNullOrEmpty(server.ServiceName)
                || string.IsNullOrEmpty(server.Host)
                || server.Port == 0)
                return;

            await _unitOfWork.MailServers.AddAsync(server);
            await _unitOfWork.CommitAsync();
            Servers.Add(server);
        }

        private void AutoSelectServer()
        {
            if (string.IsNullOrWhiteSpace(Email)) return;
            if (!EmailAddressCheck.CheckEmail(Email)) return;
            var value = Email.Split('@')[1].Split('.')[0];

            SelectedServer = Servers.SingleOrDefault(mailServer
                => mailServer.ServiceName.Contains(value, StringComparison.CurrentCultureIgnoreCase));
        }

        #endregion
    }
}