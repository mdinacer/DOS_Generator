using MaterialDesignThemes.Wpf;
using PropertyChanged;

namespace DOS_Generator.WPF.Domain
{
    [AddINotifyPropertyChangedInterface]
    public class ViewItem
    {
        public ViewItem()
        {
            IsEnabled = true;
        }

        public ViewItem(string title, object content, PackIconKind icon, bool isEnabled)
        {
            Title = title;
            Content = content;
            Icon = icon;
            IsEnabled = isEnabled;
        }

        public string Title { get; set; }
        public object Content { get; set; }
        public PackIconKind Icon { get; set; }
        public bool IsEnabled { get; set; }
    }
}