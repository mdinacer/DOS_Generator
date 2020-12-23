using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using DOS_Generator.Core;
using DOS_Generator.Core.Models;
using DOS_Generator.WPF.Services;

namespace DOS_Generator.WPF.ViewModels.Activities
{
    public class ActivitiesViewModel : ViewModelBase
    {
        #region Constructors

        public ActivitiesViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            LoadEntities();
            App.Messenger.Register("ActivitiesUpdated", LoadEntities);
        }

        #endregion

        #region Events

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            switch (propertyName)
            {
                case nameof(Date):
                case nameof(SelectedOfficer):
                case nameof(SelectedShip):
                case nameof(SelectedFacility):
                case nameof(IsReceived):
                case nameof(IsSent):
                case nameof(IsDeleted):
                    _declarations?.Refresh();
                    break;
            }
        }

        #endregion

        #region Properties

        public ObservableCollection<Declaration> Declarations { get; set; }
        public ObservableCollection<Ship> Ships { get; set; }
        public ObservableCollection<Officer> Officers { get; set; }
        public ObservableCollection<Facility> Facilities { get; set; }


        #region Filters

        public DateTime? Date { get; set; }
        public Ship SelectedShip { get; set; }
        public Officer SelectedOfficer { get; set; }
        public Facility SelectedFacility { get; set; }
        public bool IsSent { get; set; }
        public bool IsReceived { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsBusy { get; set; }
        private bool IsFiltered => CheckFiltered();

        #endregion

        private readonly IUnitOfWork _unitOfWork;
        private ICollectionView _declarations;

        #endregion

        #region Functions

        private async void LoadEntities()
        {
            IsBusy = true;
            var declarations = (await _unitOfWork.Declarations.GetAllAsync()).ToList();

            Declarations = declarations.Any()
                ? new ObservableCollection<Declaration>(declarations)
                : new ObservableCollection<Declaration>();

            _declarations = CollectionViewSource.GetDefaultView(Declarations);
            _declarations.GroupDescriptions.Add(new PropertyGroupDescription("Date.Date"));
            _declarations.SortDescriptions.Add(new SortDescription("Date.Date", ListSortDirection.Ascending));
            _declarations.SortDescriptions.Add(new SortDescription("Id", ListSortDirection.Ascending));
            _declarations.SortDescriptions.Add(new SortDescription("Ship.Name", ListSortDirection.Ascending));
            _declarations.Filter += Filter;


            var ships = declarations.Select(declaration => declaration.Ship)
                .Distinct()
                .OrderBy(ship => ship.Name).ToList();

            Ships = ships.Any() ? new ObservableCollection<Ship>(ships) : null;

            var officers = declarations.Select(declaration => declaration.Officer)
                .Distinct()
                .OrderBy(officer => officer.FullName).ToList();

            Officers = officers.Any() ? new ObservableCollection<Officer>(officers) : null;

            var facilities = declarations.Select(declaration => declaration.Facility)
                .Distinct()
                .OrderBy(facility => facility.Name).ToList();

            Facilities = facilities.Any() ? new ObservableCollection<Facility>(facilities) : null;
            IsBusy = false;
        }

        private bool Filter(object obj)
        {
            if (!(obj is Declaration declaration)) return false;
            var isDeleted = IsDeleted || !declaration.IsDeleted;
            var byShip = SelectedShip == null || declaration.ShipId == SelectedShip.Id;
            var byOfficer = SelectedOfficer == null || declaration.OfficerId == SelectedOfficer.Id;
            var byFacility = SelectedFacility == null || declaration.FacilityId == SelectedFacility.Id;
            var bySendingStatus = declaration.IsSent == IsSent;
            var byReceivingStatus = declaration.IsReceived == IsReceived;
            var byDate = Date == null || declaration.Date.Date == Date.Value.Date;

            return isDeleted && byDate && byShip && byOfficer
                   && byFacility && bySendingStatus && byReceivingStatus;
        }

        private async void SetStatus(Declaration declaration, string status)
        {
            if (declaration == null || string.IsNullOrWhiteSpace(status)) return;
            switch (status)
            {
                case "Sent":
                    declaration.IsSent = !declaration.IsSent;
                    break;
                case "Received":
                    declaration.IsReceived = !declaration.IsReceived;
                    break;
            }

            await _unitOfWork.CommitAsync();
        }

        private async void SetDeleted(Declaration declaration, bool isDeleted)
        {
            if (declaration == null) return;

            declaration.IsDeleted = isDeleted;
            await _unitOfWork.CommitAsync();
            _declarations?.Refresh();
        }

        private void ClearFilters()
        {
            Date = null;
            SelectedShip = null;
            SelectedOfficer = null;
            SelectedFacility = null;
            IsSent = IsReceived = IsDeleted = false;
            _declarations?.Refresh();
        }

        private bool CheckFiltered()
        {
            var filtersCount = 0;

            if (SelectedShip != null) filtersCount++;
            if (SelectedOfficer != null) filtersCount++;
            if (SelectedFacility != null) filtersCount++;
            if (IsSent) filtersCount++;
            if (IsReceived) filtersCount++;
            if (IsDeleted) filtersCount++;

            return filtersCount > 0;
        }

        #endregion

        #region Commands

        public ICommand ClearFiltersCommand
            => new RelayCommand(o => ClearFilters(), o => !IsBusy && IsFiltered);

        public ICommand RefreshCommand
            => new RelayCommand(o => _declarations?.Refresh(), o => !IsBusy);

        public ICommand DeleteDeclarationCommand
            => new RelayCommand<Declaration>(declaration => SetDeleted(declaration, true),
                declaration => !IsBusy && !declaration.IsDeleted);

        public ICommand RestoreDeclarationCommand
            => new RelayCommand<Declaration>(declaration => SetDeleted(declaration, false),
                declaration => !IsBusy && declaration.IsDeleted);

        public ICommand SetDeclarationSentCommand
            => new RelayCommand<Declaration>(declaration => SetStatus(declaration, "Sent"), declaration => !IsBusy);

        public ICommand SetDeclarationReceivedCommand
            => new RelayCommand<Declaration>(declaration => SetStatus(declaration, "Received"), declaration => !IsBusy && declaration.IsSent);

        #endregion
    }
}