using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using DOS_Generator.Core;
using DOS_Generator.Core.Models;
using DOS_Generator.WPF.Services;
using DOS_Generator.WPF.Views.Controls;
using DOS_Generator.WPF.Views.Dialogs;
using MaterialDesignThemes.Wpf;

namespace DOS_Generator.WPF.ViewModels.Settings
{
    public class PortsViewModel : ViewModelBase
    {
        #region Constructors

        public PortsViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            LoadData();
        }

        #endregion

        

        #region Events

        #endregion

        #region Commands

        public ICommand AddNewPortCommand
            => new RelayCommand(o => AddNewPort(), o => !IsBusy);

        public ICommand AddNewFacilityCommand
            => new RelayCommand<Port>(AddNewFacility, o => !IsBusy);

        public ICommand EditCommand
            => new RelayCommand<object>(EditItem, o => !IsBusy);

        public ICommand DeletePortCommand
            => new RelayCommand<Port>(DeleteItem, o => !IsBusy);

        public ICommand DeleteFacilityCommand
            => new RelayCommand<Facility>(DeleteItem, o => !IsBusy);

        #endregion

        #region Properties

        public ObservableCollection<Port> Ports { get; set; }
        public bool IsBusy { get; set; }
        private ICollectionView _ports;
        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region Functions

        private async void LoadData()
        {
            IsBusy = true;
            await LoadPorts();
            IsBusy = false;
        }

        private async Task LoadPorts()
        {
            var ports = (await _unitOfWork.Ports.GetAllAsync()).ToList();

            Ports = ports.Any() ? new ObservableCollection<Port>(ports) : new ObservableCollection<Port>();
            _ports = CollectionViewSource.GetDefaultView(Ports);
            _ports.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            _ports.Filter += FilterPorts;
        }

        private static bool FilterPorts(object obj)
        {
            if (!(obj is Port port)) return false;
            return !port.IsDeleted && !string.IsNullOrWhiteSpace(port.Name);
        }

        private async void EditItem(object obj)
        {
            var oldValue = obj switch
            {
                Port port => port.Name,
                Facility facility => facility.Name,
                _ => string.Empty
            };

            var isOk = (bool) await DialogHost.Show(new NameEditControl {DataContext = obj});

            if (!isOk)
                switch (obj)
                {
                    case Port port:
                        port.Name = oldValue;
                        break;
                    case Facility facility:
                        facility.Name = oldValue;
                        break;
                }
            else
                await _unitOfWork.CommitAsync();
        }

        private async void DeleteItem(object item)
        {
            if (item == null) return;

            var isOk = (bool) await DialogHost.Show(new ConfirmationDialog("Attention",
                "This element and all its related entries will be permanently delete, Proceed?"));

            if (!isOk) return;
            switch (item)
            {
                case Port port:
                    port.IsDeleted = true;
                    if (port.Facilities.Any())
                        foreach (var portFacility in port.Facilities)
                            portFacility.IsDeleted = true;
                    break;

                case Facility facility:
                    facility.IsDeleted = true;
                    break;
            }

            await _unitOfWork.CommitAsync();
        }

        private async void AddNewPort()
        {
            var port = new Port();

            var isOk = (bool) await DialogHost.Show(new NameEditControl {DataContext = port});

            if (!isOk) return;

            await _unitOfWork.Ports.AddAsync(port);
            await _unitOfWork.CommitAsync();
            Ports.Add(port);
        }

        private async void AddNewFacility(Port port)
        {
            if (port == null) return;
            var facility = new Facility{PortId = port.Id};

            var isOk = (bool) await DialogHost.Show(new NameEditControl { DataContext = facility});

            if (!isOk) return;
            port.Facilities.Add(facility);
            await _unitOfWork.CommitAsync();
        }

        #endregion
    }
}