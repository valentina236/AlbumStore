using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using AlbumStore.Helpers;
using AlbumStore.Models;
using AlbumStore.Services;
using AlbumStore.Views;

namespace AlbumStore.ViewModels
{
    public class FilterAlbumsViewModel : BaseViewModel
    {
        private readonly AlbumViewModel _parentViewModel;
        private readonly DatabaseService _databaseService;
        private string _artistNameFilter;
        private string _manufacturerNameFilter;
        private AlbumGenre _selectedGenreFilter;
        private AlbumFormat _selectedFormatFilter;
        private AlbumType _selectedTypeFilter;
        private decimal? _minPriceFilter;
        private decimal? _maxPriceFilter;
        private ObservableCollection<AlbumGenre> _genres;
        private ObservableCollection<AlbumFormat> _formats;
        private ObservableCollection<AlbumType> _types;
        private readonly System.Timers.Timer _debounceTimer = new System.Timers.Timer(300) { AutoReset = false };

        public string ArtistNameFilter
        {
            get => _artistNameFilter;
            set 
            { 
                _artistNameFilter = value; 
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public string ManufacturerNameFilter
        {
            get => _manufacturerNameFilter;
            set 
            { 
                _manufacturerNameFilter = value; 
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public AlbumGenre SelectedGenreFilter
        {
            get => _selectedGenreFilter;
            set 
            { 
                _selectedGenreFilter = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public AlbumFormat SelectedFormatFilter
        {
            get => _selectedFormatFilter;
            set 
            { 
                _selectedFormatFilter = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public AlbumType SelectedTypeFilter
        {
            get => _selectedTypeFilter;
            set 
            { 
                _selectedTypeFilter = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public decimal? MinPriceFilter
        {
            get => _minPriceFilter;
            set 
            { 
                _minPriceFilter = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public decimal? MaxPriceFilter
        {
            get => _maxPriceFilter;
            set 
            { 
                _maxPriceFilter = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public ObservableCollection<AlbumGenre> Genres
        {
            get => _genres;
            set 
            { 
                _genres = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public ObservableCollection<AlbumFormat> Formats
        {
            get => _formats;
            set 
            { 
                _formats = value; 
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public ObservableCollection<AlbumType> Types
        {
            get => _types;
            set 
            { 
                _types = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public ICommand CloseCommand { get; }
        public ICommand ClearFiltersCommand { get; }

        public FilterAlbumsViewModel(AlbumViewModel parentViewModel)
        {
            _parentViewModel = parentViewModel;
            _databaseService = new DatabaseService();
            LoadFilterData();
            CloseCommand = new RelayCommand(Close);
            ClearFiltersCommand = new RelayCommand(ClearFilters);
        }

        private void LoadFilterData()
        {
            try
            {
                Genres = _databaseService.GetGenres();
                Formats = _databaseService.GetFormats();
                Types = _databaseService.GetTypes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных фильтров: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilters()
        {
            _debounceTimer.Stop();
            _debounceTimer.Elapsed -= (s, e) => _parentViewModel.ApplyFilters(this);
            _debounceTimer.Elapsed += (s, e) => _parentViewModel.ApplyFilters(this);
            _debounceTimer.Start();
        }

        private void Close(object parameter)
        {
            if (parameter is Window window)
            {
                window.Close();
            }
        }

        private void ClearFilters(object parameter)
        {
            ArtistNameFilter = null;
            ManufacturerNameFilter = null;
            SelectedGenreFilter = null;
            SelectedFormatFilter = null;
            SelectedTypeFilter = null;
            MinPriceFilter = null;
            MaxPriceFilter = null;
        }
    }
}
