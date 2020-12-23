using System;
using System.Linq;
using System.Threading.Tasks;
using DOS_Generator.Core;
using DOS_Generator.WPF.Domain;
using DOS_Generator.WPF.Services;

namespace DOS_Generator.WPF.ViewModels.Forms
{
    public class PasswordFormViewModel : ViewModelBase
    {
        #region Constructors

        public PasswordFormViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public PasswordFormViewModel()
        {
            
        }

        #endregion

        #region Functions

        public async Task<bool> Authenticate()
        {
            var model = new LoginModel {UserName = App.User.Name, Password = OldPassword};
            var isAuthenticated = await AccountService.Authenticate(model, _unitOfWork);

            if (!isAuthenticated)
            {
                _tries++;
                if (_tries == 5)
                    App.Messenger.NotifyColleagues("Logout");

                ErrorMessage = $"Your password is incorrect, {5 - _tries} tries left";
                return false;
            }

            var account = (await _unitOfWork.Users.GetAllAsync())
                .SingleOrDefault(o => o.Name.Equals(model.UserName, StringComparison.InvariantCultureIgnoreCase));

            if (account == null) return false;

            account.Hash = AccountService.HashText($"{model.UserName}{NewPassword}");

            await _unitOfWork.CommitAsync();

            return true;
        }

        #endregion

        #region Events

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(OldPassword):
                case nameof(NewPassword):
                    ErrorMessage = null;
                    break;
            }
        }

        #endregion

        #region Properties

        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ErrorMessage { get; set; }

        private int _tries;
        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region Commands

        #endregion
    }
}