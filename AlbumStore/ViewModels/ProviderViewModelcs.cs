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
    public class ProviderViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private ObservableCollection<Provider> _providers;
        private ObservableCollection<Provider> _originalProviders;
        private Provider _selectedProvider;
        private string _searchTerm;

        public ObservableCollection<Provider> Providers
        {
            get => _providers;
            set
            {
                _providers = value;
                OnPropertyChanged(nameof(Providers));
            }
        }

        public Provider SelectedProvider
        {
            get => _selectedProvider;
            set
            {
                _selectedProvider = value;
                OnPropertyChanged(nameof(SelectedProvider));
                (EditProviderCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteProviderCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                _searchTerm = value;
                OnPropertyChanged(nameof(SearchTerm));
                FilterProviders();
            }
        }

        public ICommand AddProviderCommand { get; }
        public ICommand EditProviderCommand { get; }
        public ICommand DeleteProviderCommand { get; }
        public ICommand ClearSearchCommand { get; }

        public ProviderViewModel()
        {
            _databaseService = new DatabaseService();

            LoadProviders();

            AddProviderCommand = new RelayCommand(AddProvider);
            EditProviderCommand = new RelayCommand(EditProvider, CanEditOrDeleteProvider);
            DeleteProviderCommand = new RelayCommand(DeleteProvider, CanEditOrDeleteProvider);
            ClearSearchCommand = new RelayCommand(ClearSearch);
        }

        private void LoadProviders()
        {
            try
            {
                _originalProviders = new ObservableCollection<Provider>(_databaseService.GetProviders());
                Providers = new ObservableCollection<Provider>(_originalProviders);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки поставщиков: {ex.Message}", "Ошибка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void ClearSearch(object parameter)
        {
            SearchTerm = null;
        }

        private void FilterProviders()
        {
            var filteredList = _originalProviders.AsQueryable();
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                filteredList = filteredList.Where(p =>
                    p.ProviderName.IndexOf(SearchTerm, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (p.Address != null && p.Address.IndexOf(SearchTerm, StringComparison.OrdinalIgnoreCase) >= 0));
            }
            Providers = new ObservableCollection<Provider>(filteredList.ToList());
        }

        private void AddProvider(object parameter)
        {
            var provider = new Provider();
            var window = new AddEditProviderWindow(provider, true)
            {
                Owner = Application.Current.MainWindow
            };
            if (window.ShowDialog() == true)
            {
                Providers = _databaseService.GetProviders();
                MessageBox.Show("Поставщик успешно добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void EditProvider(object parameter)
        {
            if (SelectedProvider != null)
            {
                var providerCopy = new Provider
                {
                    ProviderCod = SelectedProvider.ProviderCod,
                    ProviderName = SelectedProvider.ProviderName,
                    Address = SelectedProvider.Address,
                    PhoneFax = SelectedProvider.PhoneFax,
                    RascetScet = SelectedProvider.RascetScet
                };
                var editWindow = new AddEditProviderWindow(providerCopy, false)
                {
                    Owner = Application.Current.MainWindow
                };
                if (editWindow.ShowDialog() == true)
                {
                    Providers = _databaseService.GetProviders();
                    MessageBox.Show("Поставщик успешно отредактирован.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void DeleteProvider(object parameter)
        {
            if (SelectedProvider != null && MessageBox.Show($"Вы уверены, что хотите удалить поставщика '{SelectedProvider.ProviderName}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    _databaseService.DeleteProvider(SelectedProvider.ProviderCod);
                    MessageBox.Show($"Поставщик '{SelectedProvider.ProviderName}' успешно удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    Providers = _databaseService.GetProviders();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении поставщика: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool CanEditOrDeleteProvider(object parameter)
        {
            return SelectedProvider != null;
        }
    }
}
