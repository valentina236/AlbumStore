using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AlbumStore.Models;
using AlbumStore.ViewModels;

namespace AlbumStore.Views
{
    /// <summary>
    /// Логика взаимодействия для AlbumView.xaml
    /// </summary>
    public partial class AlbumView : UserControl
    {
        public AlbumView()
        {
            InitializeComponent();
        }

        private void AlbumCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is Album album && DataContext is AlbumViewModel viewModel)
            {
                viewModel.SelectedAlbum = album;
                ModalBackground.Visibility = Visibility.Visible;
                e.Handled = true;
            }
        }

        private void CloseModalButton_Click(object sender, RoutedEventArgs e)
        {
            ModalBackground.Visibility = Visibility.Collapsed;
        }

        private void CloseModalBackground_Click(object sender, MouseButtonEventArgs e)
        {
            ModalBackground.Visibility = Visibility.Collapsed;
        }

        private void ModalContent_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            ModalBackground.Visibility = Visibility.Collapsed;
        }
    }
}
