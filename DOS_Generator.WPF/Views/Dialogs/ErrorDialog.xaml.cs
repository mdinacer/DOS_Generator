namespace DOS_Generator.WPF.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for ErrorDialog.xaml
    /// </summary>
    public partial class ErrorDialog
    {
        public string Message { get; set; }
        public string Title { get; set; }

        public ErrorDialog()
        {
            InitializeComponent();
        }

        public ErrorDialog(string message, string title)
        {
            Message = message;
            Title = title;
            InitializeComponent();
        }
    }
}
