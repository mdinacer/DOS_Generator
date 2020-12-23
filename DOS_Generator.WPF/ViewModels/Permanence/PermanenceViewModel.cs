using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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
using Microsoft.Extensions.DependencyInjection;

namespace DOS_Generator.WPF.ViewModels.Permanence
{
    public class PermanenceViewModel : ViewModelBase
    {
        #region Constructors

        public PermanenceViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            LoadData();
        }

        #endregion

        #region Events

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(SearchText):
                    _ships?.Refresh();
                    break;
            }
        }

        #endregion

        #region Properties

        public ObservableCollection<Ship> Ships { get; set; }
        public ObservableCollection<Declaration> Declarations { get; set; }
        public string SearchText { get; set; }
        public bool IsBusy { get; set; }
        private bool HasDeclarations => Declarations != null && Declarations.Any();

        private ICollectionView _ships;
        private bool _isEdit;
        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region Functions

        private async void LoadData()
        {
            IsBusy = true;
            await LoadShips();
            Declarations = new ObservableCollection<Declaration>();
            InitCommands();
            IsBusy = false;
        }

        private void InitCommands()
        {
            AddNewDeclarationCommand = new RelayCommand<Ship>(AddNewDeclaration, ship => ship != null && !IsBusy);

            RemoveDeclarationCommand =
                new RelayCommand<Declaration>(RemoveDeclaration, declaration => declaration != null && !IsBusy);

            EditDeclarationCommand =
                new RelayCommand<Declaration>(EditDeclaration, declaration => declaration != null && !IsBusy);

            RefreshCommand = new RelayCommand(o => RefreshShips(), o => !IsBusy);

            ClearDeclarationsCommand = new RelayCommand(o => ClearDeclarations(), o => HasDeclarations && !IsBusy);

            SendByMailCommand = new RelayCommand(o => SendDeclarations(), o => HasDeclarations && !IsBusy);

            GenerateDeclarationsCommand =
                new RelayCommand(o => GenerateDeclarations(), o => HasDeclarations && !IsBusy);

            AddNewShipCommand = new RelayCommand(o => AddNewShip());

            EditShipCommand = new RelayCommand<Ship>(EditShip);
        }

        #region Ships

        private async Task LoadShips()
        {
            var ships = (await _unitOfWork.Ships.GetAllAsync(false)).ToList();

            if (!ships.Any())
                await DialogHost.Show(new ErrorDialog("Sorry, the ships list is empty, add more ships and try again",
                    "ATTENTION"));

            Ships = ships.Any() ? new ObservableCollection<Ship>(ships) : new ObservableCollection<Ship>();

            _ships = CollectionViewSource.GetDefaultView(Ships);
            _ships.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            _ships.Filter += FilterShips;
        }

        private bool FilterShips(object obj)
        {
            if (!(obj is Ship ship)) return false;
            var bySearchText = string.IsNullOrWhiteSpace(SearchText) ||
                               ship.Name.Contains(SearchText, StringComparison.InvariantCultureIgnoreCase);
            var byVisibility = !ship.IsDeleted;
            var hasEmail = !string.IsNullOrWhiteSpace(ship.Email);
            return bySearchText && byVisibility && hasEmail;
        }

        private async void RefreshShips()
        {
            IsBusy = true;
            await LoadShips();
            if (Declarations.Any())
                foreach (var element in Declarations)
                    Ships.Remove(element.Ship);
            IsBusy = false;
        }

        private async void AddNewShip()
        {
            var viewModel = new ShipFormViewModel(_unitOfWork);
            var view = new ShipFormView {DataContext = viewModel};

            await ShowDialog(view, async (sender, args) =>
            {
                var hasErrors = viewModel.ValidateFields();
                var isOk = (bool) args.Parameter;
                if (isOk && hasErrors)
                    args.Cancel();

                if (isOk && !hasErrors) Ships.Add(await viewModel.CreateNewShip());
            });
        }

        private async void EditShip(Ship ship)
        {
            var viewModel = new ShipFormViewModel(_unitOfWork);
            viewModel.LoadShip(ship);
            var view = new ShipFormView {DataContext = viewModel};

            await ShowDialog(view, async delegate(object sender, DialogClosingEventArgs args)
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

        #endregion

        #region Declaration

        private async void SendDeclarations()
        {
            if (!Declarations.Any()) return;
            IsBusy = true;

            var view = new EmailCredentialsFormView();
            var result = await ShowDialog(view);
            if (!result) return;

            var email = view.Email;
            var password = view.PasswordBox.Password;
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) return;

            var credentials = new NetworkCredential(email, password);
            foreach (var element in Declarations)
            {
                await MailService.SendDeclaration(element, credentials);
                element.IsSent = true;
            }

            await _unitOfWork.CommitAsync();

            LoadData();
        } // TODO Review

        private async void GenerateDeclarations()
        {
            if (!Declarations.Any()) return;
            IsBusy = true;

            await SaveDeclarations();

            var files = await DeclarationCreatorService.GenerateDeclarations(Declarations.ToList());

            LoadData();
            IsBusy = false;
            if (!files.Any()) return;


            var path = Path.GetDirectoryName(files.First());
            //var path = $"{App.Settings.GeneratedDeclarationsPath}{DateTime.Today:dd.MM.yy}\\";

            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path)) return;

            files.ForEach(file =>
            {
                if(string.IsNullOrWhiteSpace(file)) return;
                Process.Start(new ProcessStartInfo
                {
                    FileName = file,
                    UseShellExecute = true,
                    Verb = "open"
                });
            });

            //Process.Start(new ProcessStartInfo
            //{
            //    FileName = path,
            //    UseShellExecute = true,
            //    Verb = "open"
            //});


        }

        private async Task SaveDeclarations()
        {
            if (!Declarations.Any()) return;
            foreach (var element in Declarations) element.Officer = App.User.Officer;

            IsBusy = true;
            await _unitOfWork.Declarations.AddRangeAsync(Declarations);
            await _unitOfWork.CommitAsync();

            App.Messenger.NotifyColleagues("ActivitiesUpdated");
            IsBusy = true;
        }

        private void AddNewDeclaration(Ship ship)
        {
            var declaration = new Declaration
                {Date = DateTime.Now, Ship = ship, StartDate = DateTime.Today};
            _isEdit = false;
            ShowDeclarationForm(declaration);
        }

        private void EditDeclaration(Declaration declaration)
        {
            _isEdit = true;
            ShowDeclarationForm(declaration);
        }

        private void RemoveDeclaration(Declaration element)
        {
            Declarations.Remove(element);
            Ships.Add(element.Ship);
        }

        private async void ShowDeclarationForm(Declaration declaration)
        {
            var viewModel = App.ServiceProvider.GetRequiredService<DeclarationFormViewModel>();
            viewModel.LoadDeclaration(declaration);
            var view = new DeclarationForm {DataContext = viewModel};

            await ShowDialog(view, (sender, args) =>
            {
                var isOk = (bool) args.Parameter;
                if (!isOk) return;
                var hasErrors = viewModel.ValidateFields();
                if (hasErrors)
                {
                    args.Cancel();
                }
                else
                {
                    viewModel.UpdateDeclaration(declaration);
                    if (_isEdit) return;
                    Ships.Remove(declaration.Ship);
                    Declarations.Add(declaration);
                }
            });
        }

        private async void ClearDeclarations()
        {
            IsBusy = true;
            Declarations.Clear();
            await LoadShips();
            IsBusy = false;
        }

        #endregion

        private static async Task<bool> ShowDialog(object view, DialogClosingEventHandler closingEventHandler = null)
        {
            return (bool) await DialogHost.Show(view, "RootDialogHost", closingEventHandler);
        }

        #endregion

        #region Commands

        public ICommand AddNewDeclarationCommand { get; set; }
        public ICommand RemoveDeclarationCommand { get; set; }
        public ICommand EditDeclarationCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand ClearDeclarationsCommand { get; set; }
        public ICommand SendByMailCommand { get; set; }
        public ICommand GenerateDeclarationsCommand { get; set; }
        public ICommand AddNewShipCommand { get; set; }
        public ICommand EditShipCommand { get; set; }

        #endregion
    }
}