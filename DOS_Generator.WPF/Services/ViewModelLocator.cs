using DOS_Generator.WPF.ViewModels;
using DOS_Generator.WPF.ViewModels.Activities;
using DOS_Generator.WPF.ViewModels.Permanence;
using DOS_Generator.WPF.ViewModels.Settings;
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

        public PermanenceViewModel PermanenceViewModel =>
            App.ServiceProvider.GetRequiredService<PermanenceViewModel>();

        public ActivitiesViewModel ActivitiesViewModel =>
            App.ServiceProvider.GetRequiredService<ActivitiesViewModel>();

        public AgenciesViewModel AgenciesViewModel =>
            App.ServiceProvider.GetRequiredService<AgenciesViewModel>();

        public UserViewModel UserViewModel =>
            App.ServiceProvider.GetRequiredService<UserViewModel>();

        public GeneralSettingsViewModel GeneralSettingsViewModel =>
            App.ServiceProvider.GetRequiredService<GeneralSettingsViewModel>();


    }
}