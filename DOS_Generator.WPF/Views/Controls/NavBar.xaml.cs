using System.Windows;

namespace DOS_Generator.WPF.Views.Controls
{
    /// <summary>
    ///     Interaction logic for NavBar.xaml
    /// </summary>
    public partial class NavBar
    {
        public static readonly DependencyProperty UserNameProperty = DependencyProperty.Register(
            "UserName", typeof(string), typeof(NavBar), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty SelectedViewProperty = DependencyProperty.Register(
            "SelectedView", typeof(object), typeof(NavBar), new PropertyMetadata(default(object)));

        public NavBar()
        {
            InitializeComponent();
        }

        public string UserName
        {
            get => (string) GetValue(UserNameProperty);
            set => SetValue(UserNameProperty, value);
        }

        public object SelectedView
        {
            get => GetValue(SelectedViewProperty);
            set => SetValue(SelectedViewProperty, value);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            App.Messenger.NotifyColleagues("Logout");
        }

        private void NavBar_OnLoaded(object sender, RoutedEventArgs e)
        {
            UserName = App.User == null ? "No user logged in" : App.User.Officer.FullName;
        }
    }
}