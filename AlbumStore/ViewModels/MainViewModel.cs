using System.Windows.Input;
using AlbumStore.Helpers;
using AlbumStore.Models;
using AlbumStore.Views;

namespace AlbumStore.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private object _currentView;
        private string _currentViewName = "MusicStore";
        private readonly User _user;

        public object CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(); }
        }

        public string CurrentViewName
        {
            get => _currentViewName;
            set
            {
                _currentViewName = value;
                OnPropertyChanged();

                OnPropertyChanged(nameof(IsAlbumsActive));
                OnPropertyChanged(nameof(IsSuppliersActive));
                OnPropertyChanged(nameof(IsMusicStoreActive));
                OnPropertyChanged(nameof(IsInvoicesActive));
                OnPropertyChanged(nameof(IsOrdersActive));
                OnPropertyChanged(nameof(IsJurnalUcetaActive));
                OnPropertyChanged(nameof(IsReportsActive));
                OnPropertyChanged(nameof(IsAdminPanelActive));
            }
        }

        public string UserRoleText
        {
            get
            {
                switch (_user.RoleName)
                {
                    case "admin":
                        return "Администратор";
                    case "seller":
                        return "Продавец";
                    case "user":
                        return "Пользователь";
                    default:
                        return "Неизвестная роль";
                }
            }
        }

        public bool IsAlbumsActive => CurrentViewName == "Albums";
        public bool IsSuppliersActive => CurrentViewName == "Suppliers";
        public bool IsMusicStoreActive => CurrentViewName == "MusicStore";
        public bool IsInvoicesActive => CurrentViewName == "Invoices";
        public bool IsOrdersActive => CurrentViewName == "Orders";
        public bool IsJurnalUcetaActive => CurrentViewName == "JurnalUceta";
        public bool IsReportsActive => CurrentViewName == "Reports";
        public bool IsAdminPanelActive => CurrentViewName == "AdminPanel";

        public bool IsAdmin => _user.RoleName == "admin";
        public bool IsAdminOrSeller => _user.RoleName == "admin" || _user.RoleName == "seller";

        public ICommand NavigateAlbumsCommand { get; }
        public ICommand NavigateSuppliersCommand { get; }
        public ICommand NavigateInvoicesCommand { get; }
        public ICommand NavigateOrdersCommand { get; }
        public ICommand NavigateAccountingCommand { get; }
        public ICommand NavigateMusicStoreCommand { get; }
        public ICommand NavigateReportsCommand { get; }
        public ICommand NavigateAdminPanelCommand { get; }

        public MainViewModel(User user)
        {
            _user = user;

            NavigateAlbumsCommand = new RelayCommand(o => NavigateToAlbums());
            NavigateSuppliersCommand = new RelayCommand(o => NavigateToSuppliers());
            NavigateInvoicesCommand = new RelayCommand(o => NavigateToInvoices());
            NavigateOrdersCommand = new RelayCommand(o => NavigateToOrders());
            NavigateAccountingCommand = new RelayCommand(o => NavigateToAccounting());
            NavigateMusicStoreCommand = new RelayCommand(o => NavigateToMusicStore());
            NavigateReportsCommand = new RelayCommand(o => NavigateToReports());
            NavigateAdminPanelCommand = new RelayCommand(o => NavigateToAdminPanel());
            NavigateToMusicStore();
        }

        private void NavigateToAlbums()
        {
            var view = new AlbumView
            {
                DataContext = new AlbumViewModel(_user)
            };
            CurrentView = view;
            CurrentViewName = "Albums";
        }

        private void NavigateToSuppliers()
        {
            var view = new ProviderView
            {
                DataContext = new ProviderViewModel()
            };
            CurrentView = view;
            CurrentViewName = "Suppliers";
        }

        private void NavigateToInvoices()
        {
            var view = new TTNView
            {
                DataContext = new TTNViewModel()
            };
            CurrentView = view;
            CurrentViewName = "Invoices";
        }

        private void NavigateToOrders()
        {
            var view = new ChekView
            {
                DataContext = new ChekViewModel(_user)
            };
            CurrentView = view;
            CurrentViewName = "Orders";
        }

        private void NavigateToAccounting()
        {
            var view = new JurnalUcetaView
            {
                DataContext = new JurnalUcetaViewModel()
            };
            CurrentView = view;
            CurrentViewName = "JurnalUceta";
        }

        private void NavigateToMusicStore()
        {
            var view = new WelcomeView
            {
                DataContext = new WelcomeView()
            };
            CurrentView = view;
            CurrentViewName = "MusicStore";
        }

        private void NavigateToReports()
        {
            var view = new ReportView
            {
                DataContext = new ReportViewModel()
            };
            CurrentView = view;
            CurrentViewName = "Reports";
        }

        private void NavigateToAdminPanel()
        {
            var view = new AdminPanelView
            {
                DataContext = new AdminPanelViewModel()
            };
            CurrentView = view;
            CurrentViewName = "AdminPanel";
        }
    }
}