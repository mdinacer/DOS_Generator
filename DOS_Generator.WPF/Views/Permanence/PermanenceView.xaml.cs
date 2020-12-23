using System.Windows;
using DOS_Generator.WPF.ViewModels.Permanence;
using Microsoft.Extensions.DependencyInjection;

namespace DOS_Generator.WPF.Views.Permanence
{
    public partial class PermanenceView
    {
        public PermanenceView()
        {
            InitializeComponent();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            DataContext = null;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = App.ServiceProvider.GetRequiredService<PermanenceViewModel>();
        }
    }
}