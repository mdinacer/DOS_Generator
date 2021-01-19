using System;
using System.Windows;
using System.Windows.Controls;
using DOS_Generator.WPF.ViewModels.Forms;

namespace DOS_Generator.WPF.Views.Forms
{
    public partial class UserFormView
    {
        public UserFormView()
        {
            InitializeComponent();
        }

        private void LoginPassword_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if(DataContext == null) return;
            if(!(DataContext is UserFormViewModel viewModel )) return;

            viewModel.UserPassword = ((PasswordBox) sender).Password;
        }

        private void MailPassword_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext == null) return;
            if (!(DataContext is UserFormViewModel viewModel)) return;

            viewModel.EmailPassword = ((PasswordBox)sender).Password;
        }
    }
}
