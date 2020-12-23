using DOS_Generator.Core.Models;
using DOS_Generator.WPF.Views;
using DOS_Generator.WPF.Views.Forms;

namespace DOS_Generator.WPF.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Constructors

        public MainWindowViewModel()
        {
            App.Messenger?.Register<User>("UserAuthenticated", SetUser);
            App.Messenger?.Register("Logout", Logout);
            View = new LoginForm();
        }

        #endregion

        #region Properties

        public object View { get; set; }

        #endregion

        #region Functions

        private void SetUser(User user)
        {
            App.User = user;
            View = new MainPageView();
        }

        private void Logout()
        {
            View = null;
            App.User = null;
            View = new LoginForm();

        }

        #endregion

        #region Events

        #endregion

        #region Commands

        #endregion
    }
}