using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DOS_Generator.Core;
using DOS_Generator.Core.Models;
using DOS_Generator.WPF.Domain;
using DOS_Generator.WPF.Services;
using Microsoft.Win32;

namespace DOS_Generator.WPF.ViewModels.Forms
{
    public class UserFormViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        #region Commands

        public ICommand BrowseCommand => new RelayCommand(o => BrowseImage());

        #endregion

        #region Properties

        public string UserName { get; set; }
        public string UserPassword { get; set; }

        #region Officer

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
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
        }

        #endregion

        #region Functions

        public void LoadUser(User user)
        {
            if (user == null) return;
            IsEdit = true;
            UserName = UserName;

            FirstName = user.Officer.FirstName;
            LastName = user.Officer.LastName;
            Title = user.Officer.Title;
            Address = user.Officer.Address;
            Phone = user.Officer.Phone;
            Email = user.Officer.Email;
            Initials = user.Officer.Initials;
            TemplatePath = user.Officer.TemplatePath;
        }

        public void UpdateUser(User user)
        {
            if (user == null) return;
            UserName = UserName;
            user.Officer.FirstName = FirstName;
            user.Officer.LastName = LastName;
            user.Officer.Title = Title;
            user.Officer.Address = Address;
            user.Officer.Phone = Phone;
            user.Officer.Email = Email;
            user.Officer.Initials = Initials;
            user.Officer.TemplatePath = TemplatePath;
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
                Email = Email,
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

            var outputFile = $".\\Resources\\{Path.GetRandomFileName()}";
            //File.Copy(TemplatePath, outputFile);
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
                                new List<object> { "This field can't be empty" };
                            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(UserName)));
                        }
                        else if ((await _unitOfWork.Users.GetAllAsync()).Any(user =>
                            user.Name.Equals(UserName, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            _validationErrorsByProperty[nameof(UserName)] =
                                new List<object> { "Another user with the same name already exists" };
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
                                new List<object> { "This field can't be empty" };
                            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(UserPassword)));
                        }
                        else if (UserPassword.Length < 4)
                        {
                            _validationErrorsByProperty[nameof(UserPassword)] =
                                new List<object> { "The password must contain more than 4 characters" };
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
                                new List<object> { "This field can't be empty" };
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
                                new List<object> { "This field can't be empty" };
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
                                new List<object> { "This field can't be empty" };
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
                                new List<object> { "This field can't be empty" };
                            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Initials)));
                        }
                        else if (_validationErrorsByProperty.Remove(nameof(Initials)))
                        {
                            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Initials)));
                        }
                        break;
                    }

                case nameof(TemplatePath):
                    {
                        if (string.IsNullOrWhiteSpace(TemplatePath))
                        {
                            _validationErrorsByProperty[nameof(TemplatePath)] =
                                new List<object> { "This field can't be empty" };
                            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(TemplatePath)));
                        }
                        else if (_validationErrorsByProperty.Remove(nameof(TemplatePath)))
                        {
                            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(TemplatePath)));
                        }
                        break;
                    }
            }
        }

        public bool ValidateFields()
        {
            Validate(nameof(UserName));
            Validate(nameof(UserPassword));
            Validate(nameof(FirstName));
            Validate(nameof(LastName));
            Validate(nameof(Title));
            Validate(nameof(Initials));
            Validate(nameof(TemplatePath));

            return HasErrors;
        }

        #endregion

        #region Events

        #endregion
    }
}