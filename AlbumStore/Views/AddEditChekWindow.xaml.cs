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
    /// Логика взаимодействия для AddEditChekWindow.xaml
    /// </summary>
    public partial class AddEditChekWindow : Window
    {
        private bool _hasValidationErrors;

        public AddEditChekWindow(Chek chek, bool isAddMode)
        {
            InitializeComponent();
            _hasValidationErrors = false;
            DataContext = new AddEditChekViewModel(chek, isAddMode);
            Loaded += AddEditChekWindow_Loaded;
        }

        private void AddEditChekWindow_Loaded(object sender, RoutedEventArgs e)
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

        private void NumberOfAlbumsTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            string numberOfAlbumsText = textBox.Text;
            if (string.IsNullOrWhiteSpace(numberOfAlbumsText))
            {
                ShowValidationError(textBox, "Количество альбомов обязательно.");
            }
            else if (!int.TryParse(numberOfAlbumsText, out int numberOfAlbums) || numberOfAlbums <= 0)
            {
                ShowValidationError(textBox, "Количество альбомов должно быть положительным числом.");
            }
            else if (numberOfAlbumsText.Length > 6)
            {
                ShowValidationError(textBox, "Количество альбомов не должно превышать 6 цифр.");
            }
            else
            {
                ClearValidationError(textBox);
            }
            UpdateSaveButtonState();
        }

        private void NumberOfAlbumsTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
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
            var viewModel = DataContext as AddEditChekViewModel;
            if (viewModel == null) return;

            _hasValidationErrors = false;

            if (viewModel.Chek.Album == null)
                _hasValidationErrors = true;

            if (string.IsNullOrWhiteSpace(viewModel.Chek.NumberOfAlbums.ToString()) || !int.TryParse(viewModel.Chek.NumberOfAlbums.ToString(), out int numberOfAlbums) || numberOfAlbums <= 0 || viewModel.Chek.NumberOfAlbums.ToString().Length > 6)
                _hasValidationErrors = true;

            var saveButton = (Button)FindName("SaveButton");
            if (saveButton != null) saveButton.IsEnabled = !_hasValidationErrors;
        }
    }
}
