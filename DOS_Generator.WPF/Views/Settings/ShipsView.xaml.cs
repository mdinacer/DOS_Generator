using System.Windows;
using System.Windows.Controls;
using DOS_Generator.WPF.ViewModels.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace DOS_Generator.WPF.Views.Settings
{
    public partial class ShipsView : UserControl
    {
        public ShipsView()
        {
            InitializeComponent();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            DataContext = null;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = App.ServiceProvider.GetRequiredService<ShipsViewModel>();
        }
    }
}