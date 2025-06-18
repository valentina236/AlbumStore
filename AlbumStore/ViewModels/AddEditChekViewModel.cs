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

namespace AlbumStore.ViewModels
{
    public class AddEditChekViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly bool _isAddMode;
        private Chek _chek;
        private ObservableCollection<Album> _albums;

        public Chek Chek
        {
            get => _chek;
            set
            {
                _chek = value;
                OnPropertyChanged(nameof(Chek));
            }
        }

        public bool IsAddMode
        {
            get => _isAddMode;
        }

        public string WindowTitle
        {
            get => $"{(IsAddMode ? "Добавление" : "Редактирование")} чека";
        }

        public ObservableCollection<Album> Albums
        {
            get => _albums;
            set
            {
                _albums = value;
                OnPropertyChanged(nameof(Albums));
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddEditChekViewModel(Chek chek, bool isAddMode)
        {
            _databaseService = new DatabaseService();
            _isAddMode = isAddMode;
            _chek = chek ?? new Chek 
            {
                Album = new Album(),
                DateOfSale = DateTime.Now,
                SaleTime = DateTime.Now.TimeOfDay
            };

            LoadComboBoxData();

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void LoadComboBoxData()
        {
            try
            {
                Albums = new ObservableCollection<Album>(_databaseService.GetAlbums());
                if (_chek.Album != null && _chek.Album.AlbumCod != 0)
                {
                    var selectedAlbum = Albums.FirstOrDefault(a => a.AlbumCod == _chek.Album.AlbumCod);
                    if (selectedAlbum != null) _chek.Album = selectedAlbum;
                    else _chek.Album = null;
                }
                else
                {
                    _chek.Album = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Save(object parameter)
        {
            try
            {
                if (_isAddMode)
                    _databaseService.AddChek(Chek);
                else
                    _databaseService.UpdateChek(Chek);

                var window = parameter as Window;
                if (window != null)
                    window.DialogResult = true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении чека: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel(object parameter)
        {
            var window = parameter as Window;
            if (window != null)
                window.DialogResult = false;
        }
    }
}
