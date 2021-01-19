using System;
using DOS_Generator.License;

namespace DOS_Generator.WPF
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            var license = App.License;
            if (license == null)
            {
                return;
            }
            var remainingDays = license.Type == LicenseType.Trial && license.ExpiryDate != null
                ? (license.ExpiryDate.Value - DateTime.Now).Days
                : 0;

            var licenseInfo = license.Type == LicenseType.Registered
                ? "Registered"
                : $"(Trial version {remainingDays} days left)";

            Title = $"DosGenerator V2 { licenseInfo}";
        }
    }
}
