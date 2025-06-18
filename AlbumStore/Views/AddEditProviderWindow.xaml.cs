using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AlbumStore.Models;
using AlbumStore.ViewModels;

namespace AlbumStore.Views
{
    /// <summary>
    /// Логика взаимодействия для AddEditProviderWindow.xaml
    /// </summary>
    public partial class AddEditProviderWindow : Window
    {
        private bool _hasValidationErrors;

        public AddEditProviderWindow(Provider provider, bool isAddMode)
        {
            InitializeComponent();
            _hasValidationErrors = false;
            DataContext = new AddEditProviderViewModel(provider, isAddMode);
            Loaded += AddEditProviderWindow_Loaded;
        }

        private void AddEditProviderWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateSaveButtonState();
        }

        private void ProviderNameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            string providerName = textBox.Text;
            if (string.IsNullOrWhiteSpace(providerName))
            {
                ShowValidationError(textBox, "Название поставщика обязательно.");
            }
            else if (providerName.Length > 50)
            {
                ShowValidationError(textBox, "Название поставщика не должно превышать 50 символов.");
            }
            else
            {
                ClearValidationError(textBox);
            }
            UpdateSaveButtonState();
        }

        private void AddressTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            string address = textBox.Text;
            if (string.IsNullOrWhiteSpace(address))
            {
                ShowValidationError(textBox, "Адрес обязателен.");
            }
            else if (address.Length > 70)
            {
                ShowValidationError(textBox, "Адрес не должен превышать 70 символов.");
            }
            else
            {
                ClearValidationError(textBox);
            }
            UpdateSaveButtonState();
        }

        private void PhoneFaxTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            string phoneFax = textBox.Text;
            if (string.IsNullOrWhiteSpace(phoneFax))
            {
                ShowValidationError(textBox, "Телефон/факс обязателен.");
            }
            else if (!Regex.IsMatch(phoneFax, @"^\+?\d{11,12}$"))
            {
                ShowValidationError(textBox, "Телефон/факс должен содержать ровно 12 символов (11 цифр и '+' или 12 цифр).");
            }
            else
            {
                ClearValidationError(textBox);
            }
            UpdateSaveButtonState();
        }

        private void PhoneFaxTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            string newText = textBox.Text + e.Text;
            e.Handled = !Regex.IsMatch(e.Text, @"[0-9+]") || newText.Length > 12;
        }

        private void RascetScetTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            string rascetScet = textBox.Text;
            if (string.IsNullOrWhiteSpace(rascetScet))
            {
                ShowValidationError(textBox, "Расчетный счет обязателен.");
            }
            else if (!Regex.IsMatch(rascetScet, @"^\d{20}$"))
            {
                ShowValidationError(textBox, "Расчетный счет должен содержать ровно 20 цифр.");
            }
            else
            {
                ClearValidationError(textBox);
            }
            UpdateSaveButtonState();
        }

        private void RascetScetTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            string newText = textBox.Text + e.Text;
            e.Handled = !Regex.IsMatch(e.Text, @"[0-9]") || newText.Length > 20;
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
            var viewModel = DataContext as AddEditProviderViewModel;
            if (viewModel == null) return;

            _hasValidationErrors = false;

            if (string.IsNullOrWhiteSpace(viewModel.Provider.ProviderName) || viewModel.Provider.ProviderName.Length > 50)
                _hasValidationErrors = true;

            if (string.IsNullOrWhiteSpace(viewModel.Provider.Address) || viewModel.Provider.Address.Length > 70)
                _hasValidationErrors = true;

            if (string.IsNullOrWhiteSpace(viewModel.Provider.PhoneFax) || !Regex.IsMatch(viewModel.Provider.PhoneFax, @"^\+?\d{11,12}$"))
                _hasValidationErrors = true;

            if (string.IsNullOrWhiteSpace(viewModel.Provider.RascetScet) || !Regex.IsMatch(viewModel.Provider.RascetScet, @"^\d{20}$"))
                _hasValidationErrors = true;

            var saveButton = (Button)FindName("SaveButton");
            if (saveButton != null) saveButton.IsEnabled = !_hasValidationErrors;
        }
    }
}
