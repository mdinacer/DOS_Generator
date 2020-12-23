using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using DOS_Generator.WPF.Domain;
using DOS_Generator.WPF.Services;
using PropertyChanged;

namespace DOS_Generator.WPF.ViewModels.Template
{
    public class DeclarationTemplateViewModel : ViewModelBase
    {
        #region Constructors

        public DeclarationTemplateViewModel()
        {
            LoadSettings();
            GenerateEntries();
            LoadDefaultEntries();
            LoadDefaultContacts();
        }

        #endregion

        #region Functions

        private void GenerateEntries()
        {
            Entries = new ObservableCollection<DosEntry>
            {
                new DosEntry
                {
                    Index = 1, IsChecked = false, Text = "Ensuring the performance of all security duties "
                },
                new DosEntry
                {
                    Index = 2,
                    IsChecked = false,
                    Text = "Monitoring restricted areas to ensure that only " +
                           "authorized personnel have access  "
                },
                new DosEntry {Index = 3, IsChecked = false, Text = "Controlling access to the port facility  "},
                new DosEntry {Index = 4, IsChecked = false, Text = "Controlling access to the ship"},
                new DosEntry
                {
                    Index = 5,
                    IsChecked = false,
                    Text = "Monitoring of the port facility, including" +
                           " berthing areas and areas surrounding the ship "
                },
                new DosEntry
                {
                    Index = 6,
                    IsChecked = false,
                    Text = "Monitoring of the ship, including berthing areas " +
                           "and areas surrounding the ship"
                },
                new DosEntry {Index = 7, IsChecked = false, Text = "Handling of cargo "},
                new DosEntry {Index = 8, IsChecked = false, Text = "Delivery of ship’s stores "},
                new DosEntry {Index = 9, IsChecked = false, Text = "Handling unaccompanied baggage"},
                new DosEntry
                {
                    Index = 10,
                    IsChecked = false,
                    Text = "Controlling the embarkation of persons and " +
                           "their effects "
                },
                new DosEntry
                {
                    Index = 11,
                    IsChecked = false,
                    Text = "Ensuring that security communication is " +
                           "readily available between the ship and port " +
                           "facility"
                }
            };
        }

        private void LoadSettings()
        {
            _settings = AppSettingsServices.ReadSettings();
        }

        private void LoadDefaultContacts()
        {
            SecurityOfficerName = _settings.SecurityOfficerName;
            Phone = _settings.Phone;
            Fax = _settings.Fax;
            Email = _settings.Email;
            Radio = _settings.Radio;
        }

        private void LoadDefaultEntries()
        {
            var entries = _settings.DefaultEntries;
            if(entries == null) return;

            foreach (var entry in Entries)
            {
                var isChecked = entries[entry.Index - 1];
                entry.IsChecked = isChecked;

            }
        }

        public bool[] GetEntries()
        {
            return Entries.OrderBy(entry => entry.Index).Select(entry => entry.IsChecked).ToArray();
        }

        private bool IsEntriesChanged()
        {
            var entries = GetEntries();
            if (_settings.DefaultEntries == null) return true;
            var changesCount =
                entries.Where((t, i) => t != _settings.DefaultEntries[i]).Count();

            return changesCount > 0;
        }

        private bool IsContactsChanged()
        {
            var changesCount = 0;

            if (SecurityOfficerName != _settings.SecurityOfficerName)
                changesCount++;

            if (Phone != _settings.Phone)
                changesCount++;

            if (Fax != _settings.Fax)
                changesCount++;

            if (Email != _settings.Email)
                changesCount++;

            if (Radio != _settings.Radio)
                changesCount++;


            return changesCount > 0;
        }

        private void SaveEntriesSettings()
        {
            var entries = GetEntries();

            if (entries != _settings.DefaultEntries)
                _settings.DefaultEntries = entries;
            AppSettingsServices.WriteSettings(_settings);
            App.Settings = _settings;
        }

        private void SaveContactsSettings()
        {
            if (!SecurityOfficerName.Equals(_settings.SecurityOfficerName))
                _settings.SecurityOfficerName = SecurityOfficerName;

            if (!Phone.Equals(_settings.Phone))
                _settings.Phone = Phone;

            if (!Fax.Equals(_settings.Fax))
                _settings.Fax = Fax;

            if (!Email.Equals(_settings.Email))
                _settings.Email = Email;

            if (!Radio.Equals(_settings.Radio))
                _settings.Radio = Radio;
            AppSettingsServices.WriteSettings(_settings);
            App.Settings = _settings;
        }

        #endregion

        #region Commands

        public ICommand SaveEntriesCommand
            => new RelayCommand(o => SaveEntriesSettings(), o => EntriesChanged);

        public ICommand ReloadEntriesCommand
            => new RelayCommand(o => LoadDefaultEntries(), o => EntriesChanged);

        public ICommand SaveContactsCommand
            => new RelayCommand(o => SaveContactsSettings(), o => ContactsChanged);

        public ICommand ReloadContactsCommand
            => new RelayCommand(o => LoadDefaultContacts(), o => ContactsChanged);

        #endregion

        #region Events

        #endregion

        #region Properties

        public ObservableCollection<DosEntry> Entries { get; set; }

        public string SecurityOfficerName { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Radio { get; set; }

        public bool EntriesChanged => IsEntriesChanged();

        public bool ContactsChanged => IsContactsChanged();

        private AppSettings _settings;

        #endregion
    }

    [AddINotifyPropertyChangedInterface]
    public class DosEntry
    {
        public int Index { get; set; }
        public string Text { get; set; }
        public bool IsChecked { get; set; }
    }
}