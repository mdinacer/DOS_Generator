namespace DOS_Generator.WPF.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for ConfirmationDialog.xaml
    /// </summary>
    public partial class ConfirmationDialog
    {
        public string Message { get; set; }
        public string Title { get; set; }

        public ConfirmationDialog()
        {
            InitializeComponent();
        }

        public ConfirmationDialog(string message, string title)
        {
            Message = message;
            Title = title;
            InitializeComponent();
        }
    }
}
