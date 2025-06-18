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
using System.Windows.Controls;

namespace AlbumStore.ViewModels
{
    public class AlbumViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private ObservableCollection<Album> _albums;
        private ObservableCollection<Album> _originalAlbums;
        private Album _selectedAlbum;
        private string _searchTerm;
        private FilterAlbumsViewModel _currentFilter;
        private FilterAlbumsWindow _filterWindow;
        private readonly User _user;

        public ObservableCollection<Album> Albums
        {
            get => _albums;
            set
            {
                _albums = value;
                OnPropertyChanged(nameof(Albums));
            }
        }

        public Album SelectedAlbum
        {
            get => _selectedAlbum;
            set { _selectedAlbum = value; OnPropertyChanged(); }
        }

        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                _searchTerm = value;
                OnPropertyChanged(nameof(SearchTerm));
                ApplySearchAndFilters();
            }
        }

        public bool IsAdmin => _user.RoleName == "admin";

        public ICommand AddAlbumCommand { get; }
        public ICommand EditAlbumCommand { get; }
        public ICommand DeleteAlbumCommand { get; }
        public ICommand ToggleFiltersCommand { get; }
        public ICommand ClearSearchCommand { get; }

        public AlbumViewModel(User user)
        {
            _user = user;
            _databaseService = new DatabaseService();

            LoadAlbums();

            AddAlbumCommand = new RelayCommand(AddAlbum);
            EditAlbumCommand = new RelayCommand(EditAlbum);
            DeleteAlbumCommand = new RelayCommand(DeleteAlbum);
            ClearSearchCommand = new RelayCommand(ClearSearch);
            ToggleFiltersCommand = new RelayCommand(ToggleFilters);
        }

        private void LoadAlbums()
        {
            try
            {
                _originalAlbums = new ObservableCollection<Album>(_databaseService.GetAlbums());
                Albums = new ObservableCollection<Album>(_originalAlbums);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка загрузки альбомов: {ex.Message}", "Ошибка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void ClearSearch(object parameter)
        {
            SearchTerm = null;
        }

        private IQueryable<Album> SearchAlbums(IQueryable<Album> albums)
        {
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                albums = albums.Where(a =>
                    a.AlbumName.IndexOf(SearchTerm, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    a.ArtistName.IndexOf(SearchTerm, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    a.Genre.GenreName.IndexOf(SearchTerm, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    a.Type.TypeName.IndexOf(SearchTerm, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    a.Format.FormatName.IndexOf(SearchTerm, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (a.ManufacturerName != null && a.ManufacturerName.IndexOf(SearchTerm, StringComparison.OrdinalIgnoreCase) >= 0));
            }
            return albums;
        }

        private IQueryable<Album> FilterAlbums(IQueryable<Album> albums)
        {
            if (_currentFilter != null)
            {
                if (!string.IsNullOrWhiteSpace(_currentFilter.ArtistNameFilter))
                {
                    albums = albums.Where(a => a.ArtistName.IndexOf(_currentFilter.ArtistNameFilter, StringComparison.OrdinalIgnoreCase) >= 0);
                }

                if (!string.IsNullOrWhiteSpace(_currentFilter.ManufacturerNameFilter))
                {
                    albums = albums.Where(a => a.ManufacturerName != null && a.ManufacturerName.IndexOf(_currentFilter.ManufacturerNameFilter, StringComparison.OrdinalIgnoreCase) >= 0);
                }

                if (_currentFilter.SelectedGenreFilter != null)
                {
                    albums = albums.Where(a => a.Genre != null && a.Genre.CodGenre == _currentFilter.SelectedGenreFilter.CodGenre);
                }

                if (_currentFilter.SelectedFormatFilter != null)
                {
                    albums = albums.Where(a => a.Format != null && a.Format.CodFormat == _currentFilter.SelectedFormatFilter.CodFormat);
                }

                if (_currentFilter.SelectedTypeFilter != null)
                {
                    albums = albums.Where(a => a.Type != null && a.Type.CodType == _currentFilter.SelectedTypeFilter.CodType);
                }

                if (_currentFilter.MinPriceFilter.HasValue)
                {
                    albums = albums.Where(a => a.UnitPrice >= _currentFilter.MinPriceFilter.Value);
                }

                if (_currentFilter.MaxPriceFilter.HasValue)
                {
                    albums = albums.Where(a => a.UnitPrice <= _currentFilter.MaxPriceFilter.Value);
                }
            }
            return albums;
        }

        private void ApplySearchAndFilters()
        {
            var filteredList = _originalAlbums.AsQueryable();
            filteredList = SearchAlbums(filteredList);
            filteredList = FilterAlbums(filteredList);
            Albums = new ObservableCollection<Album>(filteredList.ToList());
        }

        public void ApplyFilters(FilterAlbumsViewModel filter)
        {
            _currentFilter = filter;
            ApplySearchAndFilters();
        }

        private void AddAlbum(object parameter)
        {
            var album = new Album
            {
                Format = new AlbumFormat(),
                Genre = new AlbumGenre(),
                Type = new AlbumType(),
                ReleaseDate = DateTime.Now
            };
            var window = new AddEditAlbumWindow(album, true)
            {
                Owner = Application.Current.MainWindow
            };
            if (window.ShowDialog() == true)
            {
                Albums = _databaseService.GetAlbums();
                MessageBox.Show($"Альбом успешно добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void EditAlbum(object parameter)
        {
            if (parameter is Album album)
            {
                var albumCopy = new Album
                {
                    AlbumCod = album.AlbumCod,
                    AlbumName = album.AlbumName,
                    ReleaseDate = album.ReleaseDate,
                    ArtistName = album.ArtistName,
                    ManufacturerName = album.ManufacturerName,
                    Format = album.Format != null ? new AlbumFormat { CodFormat = album.Format.CodFormat, FormatName = album.Format.FormatName } : null,
                    Genre = album.Genre != null ? new AlbumGenre { CodGenre = album.Genre.CodGenre, GenreName = album.Genre.GenreName } : null,
                    Type = album.Type != null ? new AlbumType { CodType = album.Type.CodType, TypeName = album.Type.TypeName } : null,
                    UnitPrice = album.UnitPrice,
                    Photo = album.Photo
                };

                var editWindow = new AddEditAlbumWindow(albumCopy, false)
                {
                    Owner = Application.Current.MainWindow
                };

                if (editWindow.ShowDialog() == true)
                {
                    Albums = _databaseService.GetAlbums();
                    MessageBox.Show($"Альбом успешно отредактирован.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void DeleteAlbum(object parameter)
        {
            if (SelectedAlbum != null && MessageBox.Show($"Вы уверены, что хотите удалить альбом '{SelectedAlbum.AlbumName}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    _databaseService.DeleteAlbum(SelectedAlbum.AlbumCod);
                    Albums = _databaseService.GetAlbums();
                    if (parameter is UserControl userControl)
                    {
                        var modalBackground = userControl.FindName("ModalBackground") as Grid;
                        if (modalBackground != null)
                        {
                            modalBackground.Visibility = Visibility.Collapsed;
                        }
                    }
                    MessageBox.Show($"Альбом '{SelectedAlbum.AlbumName}' успешно удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении альбома: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ToggleFilters(object parameter)
        {
            if (_filterWindow == null || !_filterWindow.IsLoaded)
            {
                _filterWindow = new FilterAlbumsWindow(this)
                {
                    Owner = Application.Current.MainWindow
                };

                _filterWindow.Closed += (s, e) =>
                {
                    _filterWindow = null;
                    LoadAlbums();
                };

                _filterWindow.Show();
            }
            else
            {
                _filterWindow.Activate();
            }
        }
    }
}