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
using DOS_Generator.WPF.Views.Dialogs;
using DOS_Generator.WPF.Views.Forms;
using MaterialDesignThemes.Wpf;

namespace DOS_Generator.WPF.ViewModels.Settings
{
    public class AgenciesViewModel : ViewModelBase
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
                    await LoadAgenciesByPage(0, _pageItemsCount);
                    break;
                case nameof(SelectedPage):
                    await LoadAgenciesByPage(SelectedPage - 1, _pageItemsCount);
                    break;

                case nameof(SearchText) when string.IsNullOrWhiteSpace(SearchText):
                    _pageItemsCount = 10;
                    await LoadAgenciesByPage(0, _pageItemsCount);
                    break;
            }
        }

        #endregion

        #region Constructors

        public AgenciesViewModel()
        {
        }

        public AgenciesViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            LoadData();
        }

        #endregion

        #region Properties

        public ObservableCollection<Agency> Agencies { get; set; }
        public int SelectedPage { get; set; } = 1;
        public bool IsBusy { get; set; }
        public string SearchText { get; set; }


        private ICollectionView _agencies;
        private readonly IUnitOfWork _unitOfWork;
        private int _pagesCount;
        private int _pageItemsCount = 10;

        #endregion

        #region Functions

        private async void LoadData()
        {
            IsBusy = true;
            await LoadAgenciesByPage(0, 10);
            //await LoadShips();
            IsBusy = false;
        }

        private async Task LoadAgenciesByPage(int pageIndex, int itemsCount)
        {
            var agencies = (await _unitOfWork.Agencies.GetAllAsync())
                .OrderBy(agency => agency.Name).ToList();


            _agencies = CollectionViewSource.GetDefaultView(agencies);
            _agencies.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            _agencies.Filter += FilterAgencies;

            _pagesCount = Math.Abs(_agencies.OfType<Agency>().Count() / itemsCount) + 1;

            var agenciesPage = _agencies.OfType<Agency>().Skip(pageIndex * itemsCount).Take(itemsCount).ToList();


            Agencies = new ObservableCollection<Agency>(agenciesPage);
        }

        private bool FilterAgencies(object obj)
        {
            if (!(obj is Agency agency)) return false;
            var bySearchText = string.IsNullOrWhiteSpace(SearchText) ||
                               agency.Name.Contains(SearchText, StringComparison.InvariantCultureIgnoreCase);
            return !agency.IsDeleted && !string.IsNullOrWhiteSpace(agency.Name) && bySearchText;
        }

        private async void EditAgency(Agency agency)
        {
            if (agency == null) return;

            var view = new AgencyFormView {DataContext = agency};
            var oldValues = new Agency
            {
                Name = agency.Name,
                Email = agency.Email,
                Address = agency.Address,
                Phone = agency.Phone,
                Fax = agency.Fax,

            };

            var isOk = (bool) await DialogHost.Show(view);

            if (!isOk)
            {
                agency.Name = oldValues.Name;
                agency.Email = oldValues.Email;
                agency.Address = oldValues.Address;
                agency.Phone = oldValues.Phone;
                agency.Fax = oldValues.Fax;
                return;
            }

            await _unitOfWork.CommitAsync();
        }

        private async void RemoveAgency(Agency agency)
        {
            if (agency == null) return;

            var isOk = (bool) await DialogHost.Show(new ConfirmationDialog("Attention",
                "This element will be permanently deleted, Proceed?"));

            if (!isOk) return;

            agency.IsDeleted = true;

            await _unitOfWork.CommitAsync();
            await LoadAgenciesByPage(SelectedPage, 10);
        }

        private async void AddNewAgency()
        {
            var agency = new Agency();

            var view = new AgencyFormView {DataContext = agency};

            var isOk = (bool) await DialogHost.Show(view);

            if (!isOk) return;

            await _unitOfWork.Agencies.AddAsync(agency);
            await _unitOfWork.CommitAsync();
            SearchText = agency.Name;
            await LoadAgenciesByPage(0, 1);
        }

        #endregion

        #region Commands

        public ICommand FindAgencyCommand =>
            new RelayCommand(async o =>
            {
                _pageItemsCount = string.IsNullOrWhiteSpace(SearchText) ? 10 : 100;
                await LoadAgenciesByPage(0, _pageItemsCount);
                SelectedPage = 1;
            }, o => !IsBusy);

        public ICommand AddNewAgencyCommand =>
            new RelayCommand(o => AddNewAgency(), o => !IsBusy);

        public ICommand RefreshCommand =>
            new RelayCommand(async o => await LoadAgenciesByPage(0, 10));

        public ICommand EditAgencyCommand =>
            new RelayCommand<Agency>(EditAgency);

        public ICommand RemoveAgencyCommand =>
            new RelayCommand<Agency>(RemoveAgency);

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