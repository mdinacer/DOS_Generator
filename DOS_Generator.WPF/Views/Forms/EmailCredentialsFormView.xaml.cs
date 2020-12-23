using System.Windows;

namespace DOS_Generator.WPF.Views.Forms
{
    public partial class EmailCredentialsFormView
    {
        public static readonly DependencyProperty EmailProperty = DependencyProperty.Register(
            "Email", typeof(string), typeof(EmailCredentialsFormView), new PropertyMetadata("m.nacer.gn@gmail.com"));

        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register(
            "Password", typeof(string), typeof(EmailCredentialsFormView), new PropertyMetadata(default(string)));

        public EmailCredentialsFormView()
        {
            InitializeComponent();
        }

        public string Email
        {
            get => (string) GetValue(EmailProperty);
            set => SetValue(EmailProperty, value);
        }

        public string Password
        {
            get => (string) GetValue(PasswordProperty);
            set => SetValue(PasswordProperty, value);
        }
    }
}