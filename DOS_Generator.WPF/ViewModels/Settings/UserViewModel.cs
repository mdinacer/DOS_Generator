using System.Windows.Input;
using DOS_Generator.Core;
using DOS_Generator.Core.Models;
using DOS_Generator.WPF.Services;
using DOS_Generator.WPF.ViewModels.Forms;
using DOS_Generator.WPF.Views.Forms;
using MaterialDesignThemes.Wpf;

namespace DOS_Generator.WPF.ViewModels.Settings
{
    public class UserViewModel : ViewModelBase
    {
        #region Properties

        public User User { get; set; }
        public Officer Officer => User.Officer;

        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region Constructors

        public UserViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            User = App.User;
        }

        #endregion

        #region Events

        #endregion

        #region Functions

        private async void EditUser()
        {
            var viewModel = new UserFormViewModel(_unitOfWork);
            var view = new UserFormView{ DataContext = viewModel};

            viewModel.LoadUser(User);

            var isOk = (bool) await DialogHost.Show(view);

            if(!isOk) return;

            viewModel.UpdateUser(User);

            await _unitOfWork.CommitAsync();
            App.User = await _unitOfWork.Users.GetByIdAsync(User.Id);
        }

        private async void ResetPassword()
        {
            var viewModel = new PasswordFormViewModel(_unitOfWork);
            var view = new PasswordFormView { DataContext = viewModel };

            await DialogHost.Show(view,async delegate(object sender, DialogClosingEventArgs args)
            {
                var isOk = (bool)args.Parameter;
                if (!isOk) return;
                var isAuthenticated = await viewModel.Authenticate();
                if (!isAuthenticated)
                    args.Cancel();

            });

        }

        #endregion

        #region Commands

        public ICommand EditUserCommand => new RelayCommand(o => EditUser());
        public ICommand ResetPasswordCommand => new RelayCommand(o => ResetPassword());

        #endregion
    }
}