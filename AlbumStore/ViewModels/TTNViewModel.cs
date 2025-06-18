using System;
using System.Collections.ObjectModel;
using AlbumStore.Models;
using AlbumStore.Services;
using System.Windows;
using System.Windows.Input;
using AlbumStore.Helpers;
using AlbumStore.Views;
using System.Linq;

namespace AlbumStore.ViewModels
{
    public class TTNViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private ObservableCollection<TTN> _originalTTNs;
        private ObservableCollection<TTN> _ttns;
        private TTN _selectedTTN;
        private string _searchTerm;

        public ObservableCollection<TTN> TTNs
        {
            get => _ttns;
            set
            {
                _ttns = value;
                OnPropertyChanged(nameof(TTNs));
            }
        }

        public TTN SelectedTTN
        {
            get => _selectedTTN;
            set
            {
                _selectedTTN = value;
                OnPropertyChanged(nameof(SelectedTTN));
                (EditTTNCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteTTNCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                _searchTerm = value;
                OnPropertyChanged(nameof(SearchTerm));
                FilterTTNs();
            }
        }

        public ICommand AddTTNCommand { get; }
        public ICommand EditTTNCommand { get; }
        public ICommand DeleteTTNCommand { get; }
        public ICommand ClearSearchCommand { get; }

        public TTNViewModel()
        {
            _databaseService = new DatabaseService();
            LoadTTNs();

            AddTTNCommand = new RelayCommand(AddTTN);
            EditTTNCommand = new RelayCommand(EditTTN, CanEditOrDeleteTTN);
            DeleteTTNCommand = new RelayCommand(DeleteTTN, CanEditOrDeleteTTN);
            ClearSearchCommand = new RelayCommand(ClearSearch);
        }

        private void LoadTTNs()
        {
            try
            {
                _originalTTNs = _databaseService.GetTTNs();
                TTNs = new ObservableCollection<TTN>(_originalTTNs);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки транспортных накладных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearSearch(object parameter)
        {
            SearchTerm = null;
        }

        private void FilterTTNs()
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                TTNs = new ObservableCollection<TTN>(_originalTTNs);
                return;
            }

            var filtered = _originalTTNs.Where(ttn =>
                (ttn.DatePost != null && ttn.DatePost.ToString("dd.MM.yyyy").ToLower().Contains(SearchTerm.ToLower())) ||
                (ttn.Provider != null && ttn.Provider.ProviderName != null && ttn.Provider.ProviderName.ToLower().Contains(SearchTerm.ToLower())) ||
                (ttn.Album != null && ttn.Album.AlbumName != null && ttn.Album.AlbumName.ToLower().Contains(SearchTerm.ToLower())) ||
                (ttn.Album != null && ttn.Album.ArtistName != null && ttn.Album.ArtistName.ToLower().Contains(SearchTerm.ToLower())) ||
                (ttn.NumberOfAlbums.ToString().ToLower().Contains(SearchTerm.ToLower()))
            ).ToList();

            TTNs = new ObservableCollection<TTN>(filtered);
        }

        public void AddTTN(object parameter)
        {
            var ttn = new TTN
            {
                Provider = new Provider(),
                Album = new Album(),
                DatePost = DateTime.Now
            };
            var window = new AddEditTTNWindow(ttn, true)
            {
                Owner = Application.Current.MainWindow
            };
            if (window.ShowDialog() == true)
            {
                LoadTTNs();
                MessageBox.Show($"Накладная успешно добавлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public void EditTTN(object parameter)
        {
            if (SelectedTTN != null)
            {
                var ttnCopy = new TTN
                {
                    NumDoc = SelectedTTN.NumDoc,
                    DatePost = SelectedTTN.DatePost,
                    Provider = SelectedTTN.Provider != null ? new Provider
                    {
                        ProviderCod = SelectedTTN.Provider.ProviderCod,
                        ProviderName = SelectedTTN.Provider.ProviderName,
                        Address = SelectedTTN.Provider.Address,
                        PhoneFax = SelectedTTN.Provider.PhoneFax,
                        RascetScet = SelectedTTN.Provider.RascetScet
                    } : null,
                    Album = SelectedTTN.Album != null ? new Album
                    {
                        AlbumCod = SelectedTTN.Album.AlbumCod,
                        AlbumName = SelectedTTN.Album.AlbumName
                    } : null,
                    NumberOfAlbums = SelectedTTN.NumberOfAlbums
                };

                var editWindow = new AddEditTTNWindow(ttnCopy, false)
                {
                    Owner = Application.Current.MainWindow
                };

                if (editWindow.ShowDialog() == true)
                {
                    LoadTTNs();
                    MessageBox.Show($"Накладная успешно отредактирован.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void DeleteTTN(object parameter)
        {
            if (SelectedTTN != null && MessageBox.Show($"Вы уверены, что хотите удалить накладную №{SelectedTTN.NumDoc}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    _databaseService.DeleteTTN(SelectedTTN.NumDoc);
                    MessageBox.Show($"Накладная №{SelectedTTN.NumDoc} успешно удалена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadTTNs();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении накладной: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool CanEditOrDeleteTTN(object parameter)
        {
            return SelectedTTN != null;
        }
    }
}
