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
    public class AddEditTTNViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly bool _isAddMode;
        private TTN _ttn;
        private ObservableCollection<Provider> _providers;
        private ObservableCollection<Album> _albums;

        public TTN TTN
        {
            get => _ttn;
            set
            {
                _ttn = value;
                OnPropertyChanged(nameof(TTN));
            }
        }

        public bool IsAddMode
        {
            get => _isAddMode;
        }

        public string WindowTitle
        {
            get => $"{(IsAddMode ? "Добавление" : "Редактирование")} накладной";
        }

        public ObservableCollection<Provider> Providers
        {
            get => _providers;
            set
            {
                _providers = value;
                OnPropertyChanged(nameof(Providers));
            }
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

        public AddEditTTNViewModel(TTN ttn, bool isAddMode)
        {
            _databaseService = new DatabaseService();
            _isAddMode = isAddMode;
            _ttn = ttn ?? new TTN {
                Provider = new Provider(),
                Album = new Album(),
                DatePost = DateTime.Now
            };

            LoadComboBoxData();

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void LoadComboBoxData()
        {
            try
            {
                Providers = new ObservableCollection<Provider>(_databaseService.GetProviders());
                if (_ttn.Provider != null && _ttn.Provider.ProviderCod != 0)
                {
                    var selectedProvider = Providers.FirstOrDefault(p => p.ProviderCod == _ttn.Provider.ProviderCod);
                    if (selectedProvider != null) _ttn.Provider = selectedProvider;
                    else _ttn.Provider = null;
                }
                else
                {
                    _ttn.Provider = null;
                }

                Albums = new ObservableCollection<Album>(_databaseService.GetAlbums());
                if (_ttn.Album != null && _ttn.Album.AlbumCod != 0)
                {
                    var selectedAlbum = Albums.FirstOrDefault(a => a.AlbumCod == _ttn.Album.AlbumCod);
                    if (selectedAlbum != null) _ttn.Album = selectedAlbum;
                    else _ttn.Album = null;
                }
                else
                {
                    _ttn.Album = null;
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
                    _databaseService.AddTTN(TTN);
                else
                    _databaseService.UpdateTTN(TTN);

                var window = parameter as Window;
                if (window != null)
                    window.DialogResult = true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении накладной: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
