using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
    public class AddEditJurnalUcetaViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly bool _isAddMode;
        private JurnalUceta _jurnalEntry;
        private ObservableCollection<Album> _albums;
        private ObservableCollection<string> _months;

        public JurnalUceta JurnalEntry
        {
            get => _jurnalEntry;
            set
            {
                _jurnalEntry = value;
                OnPropertyChanged(nameof(JurnalEntry));
            }
        }

        public bool IsAddMode
        {
            get => _isAddMode;
        }

        public string WindowTitle
        {
            get => $"{(IsAddMode ? "Добавление" : "Редактирование")} записи журнала";
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

        public ObservableCollection<string> Months
        {
            get => _months;
            set
            {
                _months = value;
                OnPropertyChanged(nameof(Months));
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddEditJurnalUcetaViewModel(JurnalUceta jurnalEntry, bool isAddMode)
        {
            _databaseService = new DatabaseService();
            _isAddMode = isAddMode;
            _jurnalEntry = jurnalEntry ?? new JurnalUceta
            {
                MonthName = DateTime.Today.ToString("MMMM", CultureInfo.InvariantCulture)
            };

            Months = new ObservableCollection<string>
            {
                "January", "February", "March", "April", "May", "June",
                "July", "August", "September", "October", "November", "December"
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
                if (_jurnalEntry.Album != null && _jurnalEntry.Album.AlbumCod != 0)
                {
                    var selectedAlbum = Albums.FirstOrDefault(a => a.AlbumCod == _jurnalEntry.Album.AlbumCod);
                    if (selectedAlbum != null) _jurnalEntry.Album = selectedAlbum;
                    else _jurnalEntry.Album = null;
                }
                else
                {
                    _jurnalEntry.Album = null;
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
                    _databaseService.AddJurnalUceta(_jurnalEntry);
                else
                    _databaseService.UpdateJurnalUceta(_jurnalEntry);

                var window = parameter as Window;
                if (window != null)
                    window.DialogResult = true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении записи журнала: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
