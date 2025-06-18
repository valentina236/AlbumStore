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
using AlbumStore.ViewModels;
using AlbumStore.Models;
using Stimulsoft.Report;

namespace AlbumStore.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(User user)
        {
            InitializeComponent();

            DataContext = new MainViewModel(user);
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BtnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (WindowState == WindowState.Maximized)
                    WindowState = WindowState.Normal;
                else
                    WindowState = WindowState.Maximized;
            }
            else
            {
                if (WindowState == WindowState.Maximized)
                {
                    double percentHorizontal = e.GetPosition(this).X / ActualWidth;
                    WindowState = WindowState.Normal;

                    Left = e.GetPosition(null).X - (Width * percentHorizontal);
                }

                DragMove();
            }
        }

        /*private void NavigationButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                // Сброс стиля для всех кнопок
                ResetAllButtonStyles();

                // Установка стиля для выбранной кнопки
                SetActiveButton(button);

                // Переключение контента в зависимости от нажатой кнопки
                switch (button.Name)
                {
                    case "btnAlbums":
                        ContentArea.Content = new AlbumView();
                        break;
                    /*case "btnSuppliers":
                        ContentArea.Content = new SupplierView();
                        break;
                    case "btnInvoices":
                        ContentArea.Content = new InvoiceView();
                        break;
                    case "btnOrders":
                        ContentArea.Content = new OrderView();
                        break;
                    case "btnAccounting":
                        ContentArea.Content = new AccountingView();
                        break;
                    case "btnMusicStore":
                        ContentArea.Content = new MusicStoreView();
                        break;
                }
            }
        }*/

        /*private void ResetAllButtonStyles()
        {
            foreach (var child in ((StackPanel)((StackPanel)((Grid)((Grid)this.Content).Children[0]).Children[1]).Children[1]).Children)
            {
                if (child is Button btn)
                {
                    btn.Style = (Style)FindResource("MenuButtonStyle");
                }
            }
        }

        private void SetActiveButton(Button button)
        {
            button.Style = (Style)FindResource("SelectedMenuButtonStyle");
        }

        private void AlbumView_Loaded(object sender, RoutedEventArgs e)
        {

        }*/
    }
}
