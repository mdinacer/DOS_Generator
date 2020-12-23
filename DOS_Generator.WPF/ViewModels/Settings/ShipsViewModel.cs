using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using DOS_Generator.Core;
using DOS_Generator.Core.Models;
using DOS_Generator.WPF.Services;
using DOS_Generator.WPF.ViewModels.Forms;
using DOS_Generator.WPF.Views.Dialogs;
using DOS_Generator.WPF.Views.Forms;
using MaterialDesignThemes.Wpf;

namespace DOS_Generator.WPF.ViewModels.Settings
{
    public class ShipsViewModel : ViewModelBase
    {
        #region Events

        protected override async void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(_pageItemsCount):
                    if (_pageItemsCount > 100) _pageItemsCount = 100;
                    if (_pageItemsCount < 1) _pageItemsCount = 1;
                    await LoadShipsByPage(0, _pageItemsCount);
                    break;
                case nameof(SelectedPage):
                    await LoadShipsByPage(SelectedPage - 1, _pageItemsCount);
                    break;

                case nameof(SearchText) when string.IsNullOrWhiteSpace(SearchText):
                    _pageItemsCount = 10;
                    await LoadShipsByPage(0, _pageItemsCount);
                    break;
            }
        }

        #endregion

        #region Constructors

        public ShipsViewModel()
        {
        }

        public ShipsViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            LoadData();
        }

        #endregion

        #region Properties

        public ObservableCollection<Ship> Ships { get; set; }
        public int SelectedPage { get; set; } = 1;
        public bool IsBusy { get; set; }
        public string SearchText { get; set; }


        private ICollectionView _ships;
        private readonly IUnitOfWork _unitOfWork;
        private int _pagesCount;
        private int _pageItemsCount = 10;

        #endregion

        #region Functions

        private async void LoadData()
        {
            IsBusy = true;
            await LoadShipsByPage(0, 10);
            //await LoadShips();
            IsBusy = false;
        }

        private async Task LoadShipsByPage(int pageIndex, int itemsCount)
        {
            var ships = (await _unitOfWork.Ships.GetAllAsync())
                .OrderBy(ship => ship.Name).ToList();


            _ships = CollectionViewSource.GetDefaultView(ships);
            _ships.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            _ships.Filter += FilterShips;

            _pagesCount = Math.Abs(_ships.OfType<Ship>().Count() / itemsCount) + 1;

            var shipsPage = _ships.OfType<Ship>().Skip(pageIndex * itemsCount).Take(itemsCount).ToList();


            Ships = new ObservableCollection<Ship>(shipsPage);
        }

        private bool FilterShips(object obj)
        {
            if (!(obj is Ship ship)) return false;
            var bySearchText = string.IsNullOrWhiteSpace(SearchText) ||
                               ship.Name.Contains(SearchText, StringComparison.InvariantCultureIgnoreCase);
            return !ship.IsDeleted && !string.IsNullOrWhiteSpace(ship.Name) && bySearchText;
        }

        private async void EditShip(Ship ship)
        {
            if (ship == null) return;
            var viewModel = new ShipFormViewModel(_unitOfWork);
            viewModel.LoadShip(ship);
            var view = new ShipFormView {DataContext = viewModel};

            await DialogHost.Show(view, async delegate(object sender, DialogClosingEventArgs args)
            {
                var hasErrors = viewModel.ValidateFields();
                var isOk = (bool) args.Parameter;
                if (isOk && hasErrors)
                    args.Cancel();

                if (!isOk || hasErrors) return;
                viewModel.UpdateShip(ship);
                await _unitOfWork.CommitAsync();
            });
        }

        private async void RemoveShip(Ship ship)
        {
            if (ship == null) return;

            var isOk = (bool)
                await DialogHost.Show(new ConfirmationDialog("Attention",
                    "This element will be permanently deleted, Proceed?"));

            if (!isOk) return;

            ship.IsDeleted = true;

            await _unitOfWork.CommitAsync();
            await LoadShipsByPage(SelectedPage, 10);
        }

        private async void AddNewShip()
        {
            var viewModel = new ShipFormViewModel(_unitOfWork);
            var view = new ShipFormView {DataContext = viewModel};

            await DialogHost.Show(view, async delegate(object sender, DialogClosingEventArgs args)
            {
                var hasErrors = viewModel.ValidateFields();
                var isOk = (bool) args.Parameter;
                if (isOk && hasErrors)
                    args.Cancel();

                if (!isOk || hasErrors) return;
                var ship = await viewModel.CreateNewShip();
                Ships.Add(ship);
                SearchText = ship.Name;
                await LoadShipsByPage(0, 1);
            });
        }

        #endregion

        #region Commands

        public ICommand FindShipCommand =>
            new RelayCommand(async o =>
            {
                _pageItemsCount = string.IsNullOrWhiteSpace(SearchText) ? 10 : 100;
                await LoadShipsByPage(0, _pageItemsCount);
                SelectedPage = 1;
            }, o => !IsBusy);

        public ICommand AddNewShipCommand =>
            new RelayCommand(o => AddNewShip(), o => !IsBusy);

        public ICommand RefreshCommand =>
            new RelayCommand(async o => await LoadShipsByPage(0, 10));

        public ICommand EditShipCommand =>
            new RelayCommand<Ship>(EditShip);

        public ICommand RemoveShipCommand =>
            new RelayCommand<Ship>(RemoveShip);

        public ICommand PageFirstCommand =>
            new RelayCommand(o => SelectedPage = 1, o => SelectedPage > 1);

        public ICommand PagePreviousCommand =>
            new RelayCommand(o => SelectedPage--, o => SelectedPage > 1);

        public ICommand PageNextCommand =>
            new RelayCommand(o => SelectedPage++, o => SelectedPage < _pagesCount);

        public ICommand PageLastCommand =>
            new RelayCommand(o => SelectedPage = _pagesCount, o => SelectedPage < _pagesCount);

        #endregion
    }
}