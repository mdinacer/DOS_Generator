using System.Windows;
using DOS_Generator.WPF.ViewModels.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace DOS_Generator.WPF.Views.Settings
{
    public partial class UserView
    {
        public UserView()
        {
            InitializeComponent();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            DataContext = null;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = App.ServiceProvider.GetRequiredService<UserViewModel>();
        }
    }
}