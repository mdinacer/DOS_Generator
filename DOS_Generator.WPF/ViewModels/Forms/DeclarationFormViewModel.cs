using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using DOS_Generator.Core;
using DOS_Generator.Core.Models;

namespace DOS_Generator.WPF.ViewModels.Forms
{
    public class DeclarationFormViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        #region Events

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(SelectedPort):
                    LoadFacilities(SelectedPort.Id);
                    break;
            }
        }

        #endregion

        #region Functions

        public async void LoadDeclaration(Declaration declaration)
        {
            await LoadPorts();
            SelectedShip = declaration.Ship;
            SelectedPort = declaration.Port;
            SelectedFacility = declaration.Facility;
            SetSecurityLevel(declaration.SecLevel);
            OperationIndex = (int) declaration.Operation;
            EntryDate = declaration.StartDate;
            DepartureDate = declaration.EndDate;
        }

        public void UpdateDeclaration(Declaration declaration)
        {
            declaration.Port = SelectedPort;
            declaration.Facility = SelectedFacility;
            declaration.Operation = (OperationType) OperationIndex;
            declaration.SecLevel = GetSecurityLevel(SecurityLevelIndex);
            declaration.StartDate = EntryDate;
            declaration.EndDate = DepartureDate;
        }

        private async Task LoadPorts()
        {
            var ports = (await _unitOfWork.Ports.GetAllAsync())
                .OrderBy(port => port.Name).ToList();

            Ports = ports.Any()
                ? new ObservableCollection<Port>(ports)
                : new ObservableCollection<Port>();
        }

        private async void LoadFacilities(int portId)
        {
            var facilities = (await _unitOfWork.Facilities.GetByPortId(portId))
                .OrderBy(port => port.Name).ToList();

            Facilities = facilities.Any()
                ? new ObservableCollection<Facility>(facilities)
                : new ObservableCollection<Facility>();
        }

        private void SetSecurityLevel(string securityLevel)
        {
            SecurityLevelIndex = securityLevel switch
            {
                "Level 1" => 0,
                "Level 2" => 1,
                "Level 3" => 2,
                _ => 0
            };
        }

        private static string GetSecurityLevel(int index)
        {
            return index switch
            {
                0 => "Level 1",
                1 => "Level 2",
                2 => "Level 3",
                _ => string.Empty
            };
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (_validationErrorsByProperty.TryGetValue(propertyName, out var errors)) return errors;
            return Array.Empty<object>();
        }

        private void Validate(string changedPropertyName)
        {
            switch (changedPropertyName)
            {
                case nameof(SelectedPort):
                    if (SelectedPort == null)
                    {
                        _validationErrorsByProperty[nameof(SelectedPort)] =
                            new List<object> {"This field can't be empty"};
                        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(SelectedPort)));
                    }
                    else if (_validationErrorsByProperty.Remove(nameof(SelectedPort)))
                    {
                        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(SelectedPort)));
                    }

                    break;

                case nameof(SelectedFacility):
                    if (SelectedFacility == null)
                    {
                        _validationErrorsByProperty[nameof(SelectedFacility)] = new List<object>
                            {"This field can't be empty"};
                        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(SelectedFacility)));
                    }
                    else if (_validationErrorsByProperty.Remove(nameof(SelectedFacility)))
                    {
                        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(SelectedFacility)));
                    }

                    break;

                case nameof(OperationIndex):
                    if (OperationIndex < 0)
                    {
                        _validationErrorsByProperty[nameof(OperationIndex)] =
                            new List<object> {"This field can't be empty"};
                        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(OperationIndex)));
                    }
                    else if (_validationErrorsByProperty.Remove(nameof(OperationIndex)))
                    {
                        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(OperationIndex)));
                    }

                    break;

                case nameof(SecurityLevelIndex):
                    if (SecurityLevelIndex < 0)
                    {
                        _validationErrorsByProperty[nameof(SecurityLevelIndex)] =
                            new List<object> {"This field can't be empty"};
                        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(SecurityLevelIndex)));
                    }
                    else if (_validationErrorsByProperty.Remove(nameof(SecurityLevelIndex)))
                    {
                        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(SecurityLevelIndex)));
                    }

                    break;
            }
        }

        public bool ValidateFields()
        {
            Validate(nameof(SelectedPort));
            Validate(nameof(SelectedFacility));
            Validate(nameof(OperationIndex));
            Validate(nameof(SecurityLevelIndex));
            Validate(nameof(EntryDate));

            return HasErrors;
        }

        #endregion

        #region Properties

        #region Entities lists

        public ObservableCollection<Port> Ports { get; set; }
        public ObservableCollection<Facility> Facilities { get; set; }

        #endregion

        #region Fields

        public Ship SelectedShip { get; set; }
        public Port SelectedPort { get; set; }
        public Facility SelectedFacility { get; set; }
        public int OperationIndex { get; set; }
        public int SecurityLevelIndex { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime? DepartureDate { get; set; }

        #endregion

        #region Validation

        public bool HasErrors => _validationErrorsByProperty.Any();
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private readonly Dictionary<string, List<object>> _validationErrorsByProperty =
            new Dictionary<string, List<object>>();

        #endregion

        #region Services

        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #endregion

        #region Constructors

        public DeclarationFormViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public DeclarationFormViewModel()
        {
        }

        #endregion

        #region Commands

        #endregion
    }
}