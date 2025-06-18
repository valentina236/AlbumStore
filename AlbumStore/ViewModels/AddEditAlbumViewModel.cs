using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using AlbumStore.Helpers;
using AlbumStore.Models;
using AlbumStore.Services;
using System.Windows;
using Microsoft.Win32;
using System.Linq;
using Microsoft.SqlServer.Server;
using System.IO;
using System.Globalization;

namespace AlbumStore.ViewModels
{
    public class AddEditAlbumViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly bool _isAddMode;
        private Album _album;
        private ObservableCollection<AlbumFormat> _formats;
        private ObservableCollection<AlbumGenre> _genres;
        private ObservableCollection<AlbumType> _types;

        public Album Album
        {
            get => _album;
            set
            {
                _album = value;
                OnPropertyChanged(nameof(Album));
            }
        }

        public bool IsAddMode
        {
            get => _isAddMode;
        }

        public string WindowTitle
        {
            get => $"{(IsAddMode ? "Добавление" : "Редактирование")} альбома";
        }

        public ObservableCollection<AlbumFormat> Formats
        {
            get => _formats;
            set
            {
                _formats = value;
                OnPropertyChanged(nameof(Formats));
            }
        }

        public ObservableCollection<AlbumGenre> Genres
        {
            get => _genres;
            set
            {
                _genres = value;
                OnPropertyChanged(nameof(Genres));
            }
        }

        public ObservableCollection<AlbumType> Types
        {
            get => _types;
            set
            {
                _types = value;
                OnPropertyChanged(nameof(Types));
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand BrowsePhotoCommand { get; }

        public AddEditAlbumViewModel(Album album, bool isAddMode)
        {
            _databaseService = new DatabaseService();
            _isAddMode = isAddMode;
            _album = album ?? new Album
            {
                Format = new AlbumFormat(),
                Genre = new AlbumGenre(),
                Type = new AlbumType(),
                ReleaseDate = DateTime.Today
            };

            LoadComboBoxData();

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            BrowsePhotoCommand = new RelayCommand(BrowsePhoto);
        }

        private void LoadComboBoxData()
        {
            Formats = new ObservableCollection<AlbumFormat>(_databaseService.GetFormats());
            if(_album.Format != null && _album.Format.CodFormat != 0)
            {
                var selectedFormat = Formats.FirstOrDefault(f => f.CodFormat == _album.Format.CodFormat);
                if (selectedFormat != null)  _album.Format = selectedFormat;
                else _album.Format = null;
            }
            else _album.Format = null;
            
            Genres = new ObservableCollection<AlbumGenre>(_databaseService.GetGenres());
            if (_album.Genre != null && _album.Genre.CodGenre != 0)
            {
                var selectedGenre = Genres.FirstOrDefault(g => g.CodGenre == _album.Genre.CodGenre);
                if (selectedGenre != null) _album.Genre = selectedGenre;
                else _album.Genre = null;
            }
            else _album.Genre = null;

            Types = new ObservableCollection<AlbumType>(_databaseService.GetTypes());
            if (_album.Type != null && _album.Type.CodType != 0)
            {
                var selectedType = Types.FirstOrDefault(t => t.CodType == _album.Type.CodType);
                if (selectedType != null) _album.Type = selectedType;
                else _album.Type = null;
            }
            else _album.Type = null;
        }

        private void Save(object parameter)
        {
            try
            {
                if (_isAddMode) _databaseService.AddAlbum(Album);
                else _databaseService.UpdateAlbum(Album);

                var window = parameter as Window;
                if (window != null)
                    window.DialogResult = true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении альбома: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel(object parameter)
        {
            var window = parameter as Window;
            if (window != null)
                window.DialogResult = false;
        }

        private void BrowsePhoto(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*",
                Title = "Выберите изображение альбома"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string sourceFilePath = openFileDialog.FileName;
                    string fileName = Path.GetFileName(sourceFilePath);

                    string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                    string imagesPath = Path.Combine(projectDirectory, "Images");

                    if (!Directory.Exists(imagesPath))
                    {
                        Directory.CreateDirectory(imagesPath);
                    }

                    string destinationFilePath = Path.Combine(imagesPath, fileName);

                    string runtimeImagesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                    if (!Directory.Exists(runtimeImagesPath))
                    {
                        Directory.CreateDirectory(runtimeImagesPath);
                    }

                    string runtimeDestinationPath = Path.Combine(runtimeImagesPath, fileName);

                    File.Copy(sourceFilePath, destinationFilePath, true);

                    File.Copy(sourceFilePath, runtimeDestinationPath, true);
                    Album.Photo = fileName;
                    OnPropertyChanged(nameof(Album));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при копировании изображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
