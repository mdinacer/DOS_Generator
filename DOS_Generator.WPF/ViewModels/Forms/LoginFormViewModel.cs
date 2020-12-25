using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DOS_Generator.Core;
using DOS_Generator.WPF.Domain;
using DOS_Generator.WPF.Services;
using DOS_Generator.WPF.Views.Forms;
using MaterialDesignThemes.Wpf;

namespace DOS_Generator.WPF.ViewModels.Forms
{
    public class LoginFormViewModel : ViewModelBase
    {
        #region Events

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(UserName):
                case nameof(Password):
                    ErrorMessage = null;
                    break;
            }
        }

        #endregion

        #region Functions

        public async void Authenticate()
        {
            var user = (await _unitOfWork.Users.GetAllAsync()).SingleOrDefault(u =>
                u.Name.Equals(UserName, StringComparison.InvariantCultureIgnoreCase));

            if (user == null)
            {
                ErrorMessage = "User not found";
                return;
            }

            _isAuthenticated = await AccountService.Authenticate(new LoginModel
            {
                UserName = UserName,
                Password = Password
            }, _unitOfWork);

            ErrorMessage = _isAuthenticated ? null : "Authentication failed";

            if (_isAuthenticated) App.Messenger.NotifyColleagues("UserAuthenticated", user);
        }

        private async void AddUser()
        {
            var viewModel = new UserFormViewModel(_unitOfWork);
            var view = new UserFormView {DataContext = viewModel};
            await DialogHost.Show(view, "LoginFormDialogHost", async (sender, args) =>
            {
                var param = (bool) args.Parameter;
                
                if (!param) return;
                var hasErrors = viewModel.ValidateFields();
                if (hasErrors)
                {
                    args.Cancel();
                }
                else
                {
                    var user = await viewModel.CreateUser();
                    UserName = user.Name;
                }
            });
        }

        #endregion

        #region Properties

        public string UserName { get; set; }
        public string Password { get; set; }
        public string ErrorMessage { get; set; }

        private bool _isAuthenticated;
        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region Constructors

        public LoginFormViewModel()
        {
        }

        public LoginFormViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Commands

        public ICommand LoginCommand
            => new RelayCommand(o => Authenticate(),
                o => !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password));

        public ICommand AddUserCommand => new RelayCommand(o => AddUser());
        public ICommand ExitCommand => new RelayCommand(o => Application.Current.Shutdown());

        #endregion
    }
}