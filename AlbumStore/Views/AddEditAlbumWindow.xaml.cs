using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AlbumStore.Models;
using AlbumStore.ViewModels;

namespace AlbumStore.Views
{
    /// <summary>
    /// Логика взаимодействия для AddEditAlbumWindow.xaml
    /// </summary>
    public partial class AddEditAlbumWindow : Window
    {
        private bool _hasValidationErrors;

        public AddEditAlbumWindow(Album album, bool isAddMode)
        {
            InitializeComponent();
            _hasValidationErrors = false;
            DataContext = new AddEditAlbumViewModel(album, isAddMode);
            Loaded += AddEditAlbumWindow_Loaded;
        }

        private void AddEditAlbumWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateSaveButtonState();
        }

        private void AlbumNameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            string albumName = textBox.Text;
            if (string.IsNullOrWhiteSpace(albumName))
            {
                ShowValidationError(textBox, "Название альбома обязательно.");
            }
            else if (albumName.Length > 100)
            {
                ShowValidationError(textBox, "Название альбома не должно превышать 100 символов.");
            }
            else
            {
                ClearValidationError(textBox);
            }
            UpdateSaveButtonState();
        }

        private void ArtistNameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            string artistName = textBox.Text;
            if (string.IsNullOrWhiteSpace(artistName))
            {
                ShowValidationError(textBox, "Имя исполнителя обязательно.");
            }
            else if (artistName.Length > 100)
            {
                ShowValidationError(textBox, "Имя исполнителя не должно превышать 100 символов.");
            }
            else
            {
                ClearValidationError(textBox);
            }
            UpdateSaveButtonState();
        }

        private void PriceTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            string priceText = textBox.Text;
            if (string.IsNullOrWhiteSpace(priceText))
            {
                ShowValidationError(textBox, "Цена обязательна.");
            }
            else if (!decimal.TryParse(priceText, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal price))
            {
                ShowValidationError(textBox, "Цена должна быть числом (например, 123 или 123.45).");
            }
            else if (price <= 0)
            {
                ShowValidationError(textBox, "Цена должна быть больше 0.");
            }
            else if (price > 1000000)
            {
                ShowValidationError(textBox, "Цена не должна превышать 1,000,000.");
            }
            else
            {
                ClearValidationError(textBox);
            }
            UpdateSaveButtonState();
        }

        private void FormatComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox == null) return;

            if (comboBox.SelectedItem == null)
            {
                ShowValidationError(comboBox, "Выберите формат альбома.");
            }
            else
            {
                ClearValidationError(comboBox);
            }
            UpdateSaveButtonState();
        }

        private void GenreComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox == null) return;

            if (comboBox.SelectedItem == null)
            {
                ShowValidationError(comboBox, "Выберите жанр альбома.");
            }
            else
            {
                ClearValidationError(comboBox);
            }
            UpdateSaveButtonState();
        }

        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox == null) return;

            if (comboBox.SelectedItem == null)
            {
                ShowValidationError(comboBox, "Выберите тип альбома.");
            }
            else
            {
                ClearValidationError(comboBox);
            }
            UpdateSaveButtonState();
        }

        private void PriceTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешаем только цифры, точку и запятую
            e.Handled = !Regex.IsMatch(e.Text, @"[0-9,.]");
        }

        private void PriceTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void ManufacturerTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            string manufacturerName = textBox.Text;
            if (string.IsNullOrWhiteSpace(manufacturerName))
            {
                ShowValidationError(textBox, "Имя производителя обязательно.");
            }
            else if (manufacturerName.Length > 100)
            {
                ShowValidationError(textBox, "Имя производителя не должно превышать 100 символов.");
            }
            else
            {
                ClearValidationError(textBox);
            }
            UpdateSaveButtonState();
        }

        private void ReleaseDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var datePicker = sender as DatePicker;
            if (datePicker == null) return;

            if (!datePicker.SelectedDate.HasValue)
            {
                ShowValidationError(datePicker, "Дата выпуска обязательна.");
            }
            else if (datePicker.SelectedDate.Value > DateTime.Today)
            {
                ShowValidationError(datePicker, "Дата выпуска не может быть в будущем.");
            }
            else if (datePicker.SelectedDate.Value < new DateTime(1900, 1, 1))
            {
                ShowValidationError(datePicker, "Дата выпуска не может быть раньше 1900 года.");
            }
            else
            {
                ClearValidationError(datePicker);
            }
            UpdateSaveButtonState();
        }

        private void ShowValidationError(Control control, string message)
        {
            control.BorderBrush = System.Windows.Media.Brushes.Red;
            control.ToolTip = message;
            _hasValidationErrors = true;
        }

        private void ClearValidationError(Control control)
        {
            var brushConverter = new System.Windows.Media.BrushConverter();
            control.BorderBrush = (System.Windows.Media.Brush)brushConverter.ConvertFrom("#E9E9FF"); // Вернуть стандартный цвет
            control.ToolTip = null;
        }

        private void UpdateSaveButtonState()
        {
            // Проверяем все поля, чтобы определить, есть ли ошибки
            var viewModel = DataContext as AddEditAlbumViewModel;
            if (viewModel == null) return;

            _hasValidationErrors = false;

            if (string.IsNullOrWhiteSpace(viewModel.Album.AlbumName) || viewModel.Album.AlbumName.Length > 100)
                _hasValidationErrors = true;

            if (string.IsNullOrWhiteSpace(viewModel.Album.ArtistName) || viewModel.Album.ArtistName.Length > 100)
                _hasValidationErrors = true;

            if (!decimal.TryParse(viewModel.Album.UnitPrice.ToString(CultureInfo.InvariantCulture), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal price) ||
                price <= 0 || price > 1000000)
                _hasValidationErrors = true;

            if (viewModel.Album.Format == null || viewModel.Album.Genre == null || viewModel.Album.Type == null)
                _hasValidationErrors = true;

            if (viewModel.Album.ReleaseDate > DateTime.Today || viewModel.Album.ReleaseDate < new DateTime(1900, 1, 1))
                _hasValidationErrors = true;

            if (string.IsNullOrWhiteSpace(viewModel.Album.ManufacturerName) || viewModel.Album.ManufacturerName.Length > 100)
                _hasValidationErrors = true;

            var saveButton = (Button)FindName("SaveButton");
            if (saveButton != null) saveButton.IsEnabled = !_hasValidationErrors;
        }
    }
}
