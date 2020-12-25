using System.ComponentModel;
using System.Runtime.CompilerServices;
using DOS_Generator.WPF.Annotations;

namespace DOS_Generator.WPF.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Properties

        #endregion

        #region Constructors

        #endregion

        #region Functions

        #endregion

        #region Events

        #endregion

        #region Commands

        #endregion


    }
}