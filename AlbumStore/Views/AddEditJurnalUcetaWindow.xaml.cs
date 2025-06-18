using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для AddEditJurnalUcetaWindow.xaml
    /// </summary>
    public partial class AddEditJurnalUcetaWindow : Window
    {
        private bool _hasValidationErrors;

        public AddEditJurnalUcetaWindow(JurnalUceta jurnalEntry, bool isAddMode)
        {
            InitializeComponent();
            _hasValidationErrors = false;
            DataContext = new AddEditJurnalUcetaViewModel(jurnalEntry, isAddMode);
            Loaded += AddEditJurnalUcetaWindow_Loaded;
        }

        private void AddEditJurnalUcetaWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateSaveButtonState();
        }

        private void AlbumComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox == null) return;

            if (comboBox.SelectedItem == null)
            {
                ShowValidationError(comboBox, "Выберите альбом.");
            }
            else
            {
                ClearValidationError(comboBox);
            }
            UpdateSaveButtonState();
        }

        private void NumbeOfDeliveredAlbumsTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            string numberText = textBox.Text;
            if (string.IsNullOrWhiteSpace(numberText))
            {
                ShowValidationError(textBox, "Количество поставленных альбомов обязательно.");
            }
            else if (!int.TryParse(numberText, out int number) || number <= 0)
            {
                ShowValidationError(textBox, "Количество поставленных альбомов должно быть положительным числом.");
            }
            else if (numberText.Length > 6)
            {
                ShowValidationError(textBox, "Количество поставленных альбомов не должно превышать 6 цифр.");
            }
            else
            {
                ClearValidationError(textBox);
            }
            UpdateSaveButtonState();
        }

        private void NumbeOfDeliveredAlbumsTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            string newText = textBox.Text + e.Text;
            e.Handled = !Regex.IsMatch(e.Text, @"[0-9]") || newText.Length > 6;
        }

        private void NumberOfAlbumsSoldTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            string numberText = textBox.Text;
            if (string.IsNullOrWhiteSpace(numberText))
            {
                ShowValidationError(textBox, "Количество проданных альбомов обязательно.");
            }
            else if (!int.TryParse(numberText, out int number) || number <= 0)
            {
                ShowValidationError(textBox, "Количество проданных альбомов должно быть положительным числом.");
            }
            else if (numberText.Length > 6)
            {
                ShowValidationError(textBox, "Количество проданных альбомов не должно превышать 6 цифр.");
            }
            else
            {
                ClearValidationError(textBox);
            }
            UpdateSaveButtonState();
        }

        private void NumberOfAlbumsSoldTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            string newText = textBox.Text + e.Text;
            e.Handled = !Regex.IsMatch(e.Text, @"[0-9]") || newText.Length > 6;
        }

        private void ShowValidationError(Control control, string message)
        {
            control.BorderBrush = Brushes.Red;
            control.ToolTip = message;
            _hasValidationErrors = true;
        }

        private void ClearValidationError(Control control)
        {
            var brushConverter = new BrushConverter();
            control.BorderBrush = (Brush)brushConverter.ConvertFrom("#E9E9FF");
            control.ToolTip = null;
        }

        private void UpdateSaveButtonState()
        {
            var viewModel = DataContext as AddEditJurnalUcetaViewModel;
            if (viewModel == null) return;

            _hasValidationErrors = false;

            if (viewModel.JurnalEntry.Album == null)
            {
                _hasValidationErrors = true;
            }

            if (string.IsNullOrWhiteSpace(viewModel.JurnalEntry.NumbeOfDeliveredAlbums.ToString()) ||
                !int.TryParse(viewModel.JurnalEntry.NumbeOfDeliveredAlbums.ToString(), out int delivered) ||
                delivered <= 0 ||
                viewModel.JurnalEntry.NumbeOfDeliveredAlbums.ToString().Length > 6)
            {
                _hasValidationErrors = true;
            }

            if (string.IsNullOrWhiteSpace(viewModel.JurnalEntry.NumberOfAlbumsSold.ToString()) ||
                !int.TryParse(viewModel.JurnalEntry.NumberOfAlbumsSold.ToString(), out int sold) ||
                sold <= 0 ||
                viewModel.JurnalEntry.NumberOfAlbumsSold.ToString().Length > 6)
            {
                _hasValidationErrors = true;
            }

            var saveButton = (Button)FindName("SaveButton");
            if (saveButton != null) saveButton.IsEnabled = !_hasValidationErrors;
        }
    }
}
