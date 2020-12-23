using DOS_Generator.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace DOS_Generator.WPF.Services
{
    public class ViewModelLocator
    {
        public MainWindowViewModel MainWindowViewModel =>
            App.ServiceProvider.GetRequiredService<MainWindowViewModel>();

        
    }
}