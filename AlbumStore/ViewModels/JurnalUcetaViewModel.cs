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
using System.Globalization;
using System.Windows.Navigation;

namespace AlbumStore.ViewModels
{
    public class JurnalUcetaViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private ObservableCollection<JurnalUceta> _jurnalEntries;
        private ObservableCollection<JurnalUceta> _originalJurnalEntries;
        private JurnalUceta _selectedJurnalEntry;
        private string _searchTerm;

        public ObservableCollection<JurnalUceta> JurnalEntries
        {
            get => _jurnalEntries;
            set
            {
                _jurnalEntries = value;
                OnPropertyChanged(nameof(JurnalEntries));
            }
        }

        public JurnalUceta SelectedJurnalEntry
        {
            get => _selectedJurnalEntry;
            set
            {
                _selectedJurnalEntry = value;
                OnPropertyChanged(nameof(SelectedJurnalEntry));
                (EditJurnalEntryCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteJurnalEntryCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                _searchTerm = value;
                OnPropertyChanged(nameof(SearchTerm));
                FilterJurnalEntries();
            }
        }

        public ICommand AddJurnalEntryCommand { get; }
        public ICommand EditJurnalEntryCommand { get; }
        public ICommand DeleteJurnalEntryCommand { get; }
        public ICommand ClearSearchCommand { get; }

        public JurnalUcetaViewModel()
        {
            _databaseService = new DatabaseService();
            LoadJurnalEntries();

            AddJurnalEntryCommand = new RelayCommand(AddJurnalEntry);
            EditJurnalEntryCommand = new RelayCommand(EditJurnalEntry, CanEditOrDeleteJurnalEntry);
            DeleteJurnalEntryCommand = new RelayCommand(DeleteJurnalEntry, CanEditOrDeleteJurnalEntry);
            ClearSearchCommand = new RelayCommand(ClearSearch);
        }

        private void LoadJurnalEntries()
        {
            try
            {
                _originalJurnalEntries = _databaseService.GetJurnalUceta();
                JurnalEntries = new ObservableCollection<JurnalUceta>(_originalJurnalEntries);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки записей журнала: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearSearch(object parameter)
        {
            SearchTerm = null;
        }

        private void FilterJurnalEntries()
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                JurnalEntries = new ObservableCollection<JurnalUceta>(_originalJurnalEntries);
                return;
            }

            var filtered = _originalJurnalEntries.Where(journalEntry =>
                (journalEntry.MonthName != null && journalEntry.MonthName.ToLower().Contains(SearchTerm.ToLower())) ||
                (journalEntry.Album != null && journalEntry.Album.AlbumName != null && journalEntry.Album.AlbumName.ToLower().Contains(SearchTerm.ToLower())) ||
                (journalEntry.Album != null && journalEntry.Album.ArtistName != null && journalEntry.Album.ArtistName.ToLower().Contains(SearchTerm.ToLower())) ||
                (journalEntry.NumbeOfDeliveredAlbums.ToString().ToLower().Contains(SearchTerm.ToLower())) ||
                (journalEntry.NumberOfAlbumsSold.ToString().ToLower().Contains(SearchTerm.ToLower()))
            ).ToList();

            JurnalEntries = new ObservableCollection<JurnalUceta>(filtered);
        }

        private void AddJurnalEntry(object parameter)
        {
            var jurnalEntry = new JurnalUceta
            {
                MonthName = DateTime.Today.ToString("MMMM", CultureInfo.InvariantCulture)
            };
            var window = new AddEditJurnalUcetaWindow(jurnalEntry, true)
            {
                Owner = Application.Current.MainWindow
            };
            if (window.ShowDialog() == true)
            {
                LoadJurnalEntries();
                MessageBox.Show("Запись журнала успешно добавлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void EditJurnalEntry(object parameter)
        {
            if (SelectedJurnalEntry != null)
            {
                var jurnalEntryCopy = new JurnalUceta
                {
                    MonthName = SelectedJurnalEntry.MonthName,
                    Album = SelectedJurnalEntry.Album != null ? new Album
                    {
                        AlbumCod = SelectedJurnalEntry.Album.AlbumCod,
                        AlbumName = SelectedJurnalEntry.Album.AlbumName,
                        ArtistName = SelectedJurnalEntry.Album.ArtistName
                    } : null,
                    NumbeOfDeliveredAlbums = SelectedJurnalEntry.NumbeOfDeliveredAlbums,
                    NumberOfAlbumsSold = SelectedJurnalEntry.NumberOfAlbumsSold
                };

                var editWindow = new AddEditJurnalUcetaWindow(jurnalEntryCopy, false)
                {
                    Owner = Application.Current.MainWindow
                };

                if (editWindow.ShowDialog() == true)
                {
                    LoadJurnalEntries();
                    MessageBox.Show("Запись журнала успешно отредактирована.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void DeleteJurnalEntry(object parameter)
        {
            if (SelectedJurnalEntry != null && MessageBox.Show($"Вы уверены, что хотите удалить запись за месяц '{SelectedJurnalEntry.MonthName}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    _databaseService.DeleteJurnalUceta(SelectedJurnalEntry.MonthName, SelectedJurnalEntry.Album.AlbumName);
                    LoadJurnalEntries();
                    MessageBox.Show("Запись журнала успешно удалена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении записи журнала: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool CanEditOrDeleteJurnalEntry(object parameter)
        {
            return SelectedJurnalEntry != null;
        }
    }
}
