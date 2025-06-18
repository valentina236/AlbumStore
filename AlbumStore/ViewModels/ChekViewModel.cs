using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlbumStore.Helpers;
using AlbumStore.Models;
using AlbumStore.Services;
using System.Windows.Input;
using System.Windows;
using AlbumStore.Views;

namespace AlbumStore.ViewModels
{
    public class ChekViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private ObservableCollection<Chek> _originalCheks;
        private ObservableCollection<Chek> _cheks;
        private Chek _selectedChek;
        private readonly User _user;
        private string _searchTerm;

        public ObservableCollection<Chek> Cheks
        {
            get => _cheks;
            set
            {
                _cheks = value;
                OnPropertyChanged(nameof(Cheks));
            }
        }

        public Chek SelectedChek
        {
            get => _selectedChek;
            set
            {
                _selectedChek = value;
                OnPropertyChanged(nameof(SelectedChek));
                (EditChekCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteChekCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                _searchTerm = value;
                OnPropertyChanged(nameof(SearchTerm));
                FilterCheks();
            }
        }

        public bool IsAdmin => _user.RoleName == "admin";

        public ICommand AddChekCommand { get; }
        public ICommand EditChekCommand { get; }
        public ICommand DeleteChekCommand { get; }
        public ICommand ClearSearchCommand { get; }

        public ChekViewModel(User user)
        {
            _user = user;
            _databaseService = new DatabaseService();
            LoadCheks();

            AddChekCommand = new RelayCommand(AddChek);
            EditChekCommand = new RelayCommand(EditChek, CanEditOrDeleteChek);
            DeleteChekCommand = new RelayCommand(DeleteChek, CanEditOrDeleteChek);
            ClearSearchCommand = new RelayCommand(ClearSearch);
        }

        private void LoadCheks()
        {
            try
            {
                _originalCheks = _databaseService.GetCheks();
                Cheks = new ObservableCollection<Chek>(_originalCheks);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки чеков: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearSearch(object parameter)
        {
            SearchTerm = null;
        }

        private void FilterCheks()
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                Cheks = new ObservableCollection<Chek>(_originalCheks);
                return;
            }

            var filtered = _originalCheks.Where(check =>
                (check.DateOfSale != null && check.DateOfSale.ToString("dd.MM.yyyy").ToLower().Contains(SearchTerm.ToLower())) ||
                (check.Album != null && check.Album.AlbumName != null && check.Album.AlbumName.ToLower().Contains(SearchTerm.ToLower())) ||
                (check.Album != null && check.Album.ArtistName != null && check.Album.ArtistName.ToLower().Contains(SearchTerm.ToLower())) ||
                (check.NumberOfAlbums.ToString().ToLower().Contains(SearchTerm.ToLower()))
            ).ToList();

            Cheks = new ObservableCollection<Chek>(filtered);
        }

        private void AddChek(object parameter)
        {
            var chek = new Chek
            {
                Album = new Album(),
                DateOfSale = DateTime.Now,
                SaleTime = DateTime.Now.TimeOfDay
            };
            var window = new AddEditChekWindow(chek, true)
            {
                Owner = Application.Current.MainWindow
            };
            if (window.ShowDialog() == true)
            {
                LoadCheks();
                MessageBox.Show("Чек успешно добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void EditChek(object parameter)
        {
            if (SelectedChek != null)
            {
                var chekCopy = new Chek
                {
                    CheckNumber = SelectedChek.CheckNumber,
                    DateOfSale = SelectedChek.DateOfSale,
                    SaleTime = SelectedChek.SaleTime,
                    Album = SelectedChek.Album != null ? new Album
                    {
                        AlbumCod = SelectedChek.Album.AlbumCod,
                        AlbumName = SelectedChek.Album.AlbumName,
                        ArtistName = SelectedChek.Album.ArtistName
                    } : null,
                    NumberOfAlbums = SelectedChek.NumberOfAlbums
                };

                var editWindow = new AddEditChekWindow(chekCopy, false)
                {
                    Owner = Application.Current.MainWindow
                };

                if (editWindow.ShowDialog() == true) 
                {
                    LoadCheks();
                    MessageBox.Show("Чек успешно отредактирован.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void DeleteChek(object parameter)
        {
            if (SelectedChek != null && MessageBox.Show($"Вы уверены, что хотите удалить чек с номером '{SelectedChek.CheckNumber}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    _databaseService.DeleteChek(SelectedChek.CheckNumber);
                    LoadCheks();
                    MessageBox.Show("Чек успешно удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении чека: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool CanEditOrDeleteChek(object parameter)
        {
            return SelectedChek != null;
        }
    }
}