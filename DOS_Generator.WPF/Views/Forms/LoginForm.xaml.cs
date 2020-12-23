using System.Windows;
using System.Windows.Controls;
using DOS_Generator.Core;
using DOS_Generator.WPF.ViewModels.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace DOS_Generator.WPF.Views.Forms
{
    public partial class LoginForm
    {
        public LoginForm()
        {
            InitializeComponent();
            DataContext = new LoginFormViewModel(App.ServiceProvider.GetService<IUnitOfWork>());
        }

        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if(!(DataContext is LoginFormViewModel viewModel)) return;
            viewModel.Password = ((PasswordBox)sender).Password; // Todo Implement a safer way to manage passwords
        }
    }
}
