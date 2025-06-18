using System;
using System.Windows;
using System.Windows.Input;
using AlbumStore.Helpers;
using AlbumStore.Services;
using AlbumStore.Views;

namespace AlbumStore.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private string _username;
        private string _password;
        private bool _isPasswordVisible;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
                (LoginCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
                (LoginCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public bool IsPasswordVisible
        {
            get => _isPasswordVisible;
            set
            {
                _isPasswordVisible = value;
                OnPropertyChanged(nameof(IsPasswordVisible));
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand CloseCommand { get; }

        public LoginViewModel()
        {
            _databaseService = new DatabaseService();
            LoginCommand = new RelayCommand(Login, CanLogin);
            RegisterCommand = new RelayCommand(Register);
            CloseCommand = new RelayCommand(Close);
        }

        private void Login(object parameter)
        {
            var user = _databaseService.AuthenticateUser(Username, Password);
            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                var mainWindow = new MainWindow(user);
                mainWindow.Show();
                Application.Current.Windows[0].Close();
            }
        }

        private bool CanLogin(object parameter)
        {
            return !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);
        }

        private void Register(object parameter)
        {
            MessageBox.Show("Открыть окно регистрации");
        }

        private void Close(object parameter)
        {
            Application.Current.Shutdown();
        }
    }
}