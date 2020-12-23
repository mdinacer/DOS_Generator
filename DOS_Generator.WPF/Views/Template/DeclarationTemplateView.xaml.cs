using System.Windows;
using DOS_Generator.WPF.ViewModels.Template;
using Microsoft.Extensions.DependencyInjection;

namespace DOS_Generator.WPF.Views.Template
{
    public partial class DeclarationTemplateView
    {
        public DeclarationTemplateView()
        {
            InitializeComponent();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            DataContext = null;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = App.ServiceProvider.GetRequiredService<DeclarationTemplateViewModel>();
        }
    }
}