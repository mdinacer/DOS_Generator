using DOS_Generator.WPF.ViewModels;
using DOS_Generator.WPF.ViewModels.Template;
using Microsoft.Extensions.DependencyInjection;

namespace DOS_Generator.WPF.Services
{
    public class ViewModelLocator
    {
        public MainWindowViewModel MainWindowViewModel =>
            App.ServiceProvider.GetRequiredService<MainWindowViewModel>();

        public DeclarationTemplateViewModel DeclarationTemplateViewModel =>
            App.ServiceProvider.GetRequiredService<DeclarationTemplateViewModel>();


    }
}