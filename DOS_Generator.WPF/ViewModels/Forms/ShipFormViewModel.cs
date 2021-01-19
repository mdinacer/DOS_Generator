using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DOS_Generator.Core;
using DOS_Generator.Core.Models;
using DOS_Generator.WPF.Helpers;
using DOS_Generator.WPF.Services;

namespace DOS_Generator.WPF.ViewModels.Forms
{
    public class ShipFormViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        #region Properties

        public ObservableCollection<Agency> Agencies { get; set; }
        public bool IsNewAgency { get; set; }
        public bool IsAgencyFormClosed => !IsNewAgency;
        public bool IsEdit { get; set; }

        #region Fields

        public string Name { get; set; }
        public string ImoNumber { get; set; }
        public string PortOfRegistry { get; set; }
        public string Email { get; set; }
        public Agency SelectedAgency { get; set; }

        #region NewAgency

        public string AgencyName { get; set; }
        public string AgencyEmail { get; set; }

        #endregion

        #endregion

        #region Validation

        public bool HasErrors => _validationErrorsByProperty.Any();
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private readonly Dictionary<string, List<object>> _validationErrorsByProperty =
            new Dictionary<string, List<object>>();

        #endregion

        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region Constructors

        public ShipFormViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            LoadAgencies();
        }

        public ShipFormViewModel()
        {
        }

        #endregion

        #region Functions

        private async void LoadAgencies()
        {
            var agencies = (await _unitOfWork.Agencies.GetAllAsync())
                .OrderBy(agency => agency.Name).ToList();

            Agencies = agencies.Any()
                ? new ObservableCollection<Agency>(agencies)
                : new ObservableCollection<Agency>();
        }

        private void CreateAgency()
        {
            var newAgency = new Agency
            {
                Name = AgencyName,
                Email = AgencyEmail
            };
            Agencies.Add(newAgency);
            SelectedAgency = newAgency;
        }

        public void LoadShip(Ship ship)
        {
            if (ship == null) return;
            IsEdit = true;
            Name = ship.Name;
            ImoNumber = ship.ImoNumber;
            PortOfRegistry = ship.PortOfRegistry;
            Email = ship.Email;
            SelectedAgency = ship.Agency;
        }

        private Ship GenerateShip()
        {
            return new Ship
            {
                Name = Name,
                ImoNumber = ImoNumber,
                PortOfRegistry = PortOfRegistry,
                Email = Email,
                Agency = SelectedAgency
            };
        }

        public async Task<Ship> CreateNewShip()
        {
            var ship = GenerateShip();
            await _unitOfWork.Ships.AddAsync(ship);
            await _unitOfWork.CommitAsync();
            return ship;
        }

        public void UpdateShip(Ship ship)
        {
            ship.Name = Name;
            ship.ImoNumber = ImoNumber;
            ship.PortOfRegistry = PortOfRegistry;
            ship.Email = Email;
            ship.Agency = SelectedAgency;
        }

        public async Task<Ship> UpdateShip(int id)
        {
            var ship = GenerateShip();
            ship.Id = id;
            _unitOfWork.Ships.Update(ship);
            await _unitOfWork.CommitAsync();
            return ship;
        }

        #region Validation

        public IEnumerable GetErrors(string propertyName)
        {
            if (_validationErrorsByProperty.TryGetValue(propertyName, out var errors)) return errors;
            return Array.Empty<object>();
        }

        private async void Validate(string changedPropertyName)
        {
            switch (changedPropertyName)
            {
                case nameof(Name) when !IsEdit:
                {
                    if (string.IsNullOrWhiteSpace(Name))
                    {
                        _validationErrorsByProperty[nameof(Name)] =
                            new List<object> {"This field can't be empty"};
                        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Name)));
                    }
                    else if ((await _unitOfWork.Ships.GetAllAsync()).Any(ship =>
                        ship.Name.Equals(Name, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        _validationErrorsByProperty[nameof(Name)] =
                            new List<object> {"Another ship with the same name already exists"};
                        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Name)));
                    }
                    else if (_validationErrorsByProperty.Remove(nameof(Name)))
                    {
                        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Name)));
                    }

                    break;
                }

                case nameof(Email) when !IsEdit:
                {
                    if (string.IsNullOrWhiteSpace(Email))
                    {
                        _validationErrorsByProperty[nameof(Email)] =
                            new List<object> {"This field can't be empty"};
                        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Email)));
                    }
                    else if ((await _unitOfWork.Ships.GetAllAsync()).Any(ship
                        => !string.IsNullOrWhiteSpace(ship.Email) &&
                           ship.Email.Equals(Email, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        _validationErrorsByProperty[nameof(Email)] =
                            new List<object> {"Another ship with the same Email already exists"};
                        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Email)));
                    }
                    else if (_validationErrorsByProperty.Remove(nameof(Email)))
                    {
                        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Email)));
                    }

                    break;
                }

                case nameof(AgencyName) when IsNewAgency:
                {
                    if (string.IsNullOrWhiteSpace(AgencyName))
                    {
                        _validationErrorsByProperty[nameof(AgencyName)] =
                            new List<object> {"This field can't be empty"};
                        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(AgencyName)));
                    }
                    else if ((await _unitOfWork.Agencies.GetAllAsync()).Any(agency =>
                        agency.Name.Equals(AgencyName, StringComparison.CurrentCulture)))
                    {
                        _validationErrorsByProperty[nameof(AgencyName)] =
                            new List<object> {"Another Agency with the same name already exists"};
                        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(AgencyName)));
                    }
                    else if (_validationErrorsByProperty.Remove(nameof(AgencyName)))
                    {
                        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(AgencyName)));
                    }

                    break;
                }

                case nameof(AgencyEmail) when IsNewAgency:
                {
                    if (string.IsNullOrWhiteSpace(AgencyEmail))
                    {
                        _validationErrorsByProperty[nameof(AgencyEmail)] =
                            new List<object> {"This field can't be empty"};
                        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(AgencyEmail)));
                    }
                    else if (! EmailAddressCheck.CheckEmail(AgencyEmail))
                    {
                        _validationErrorsByProperty[nameof(AgencyEmail)] =
                            new List<object> {"Not valid email address"};
                        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(AgencyEmail)));
                    }
                    else if (_validationErrorsByProperty.Remove(nameof(AgencyEmail)))
                    {
                        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(AgencyEmail)));
                    }

                    break;
                }
            }
        }

        public bool ValidateFields()
        {
            Validate(nameof(Name));
            Validate(nameof(Email));
            return HasErrors;
        }

        private bool ValidateNewAgency()
        {
            Validate(nameof(AgencyName));
            Validate(nameof(AgencyEmail));
            return HasErrors;
        }

        #endregion

        #endregion

        #region Events

        #endregion

        #region Commands

        public ICommand AddNewAgencyCommand =>
            new RelayCommand(o => IsNewAgency = true);

        public ICommand CreateAgencyCommand =>
            new RelayCommand(o =>
            {
                if (ValidateNewAgency()) return;
                CreateAgency();
                IsNewAgency = false;
                AgencyName = AgencyEmail = string.Empty;
            });

        public ICommand CancelNewAgencyCommand =>
            new RelayCommand(o =>
            {
                AgencyName = AgencyEmail = string.Empty;
                IsNewAgency = false;
            });

        public ICommand SearchCommand => new RelayCommand(o => Process.Start(new ProcessStartInfo
        {
            FileName = $"https://www.q88.com/viewship.aspx?vessel={Name.ToLower()}",
            UseShellExecute = true,
            Verb = "open"
        }), o => !string.IsNullOrWhiteSpace(Name));

        #endregion
    }
}