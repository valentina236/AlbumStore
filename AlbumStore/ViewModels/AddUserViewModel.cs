using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlbumStore.Helpers;
using System.Windows.Input;
using System.Windows;
using System.Text.RegularExpressions;

namespace AlbumStore.ViewModels
{
    public class AddUserViewModel : BaseViewModel
    {
        private string _userName;
        private string _password;
        private string _roleName;

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged(nameof(UserName));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public string RoleName
        {
            get => _roleName;
            set
            {
                _roleName = value;
                OnPropertyChanged(nameof(RoleName));
            }
        }

        public ICommand AddCommand { get; }
        public ICommand CancelCommand { get; }

        public Action<bool> CloseAction { get; set; }

        public AddUserViewModel()
        {
            AddCommand = new RelayCommand(o => Add());
            CancelCommand = new RelayCommand(o => Cancel());
        }

        private void Add()
        {

            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(RoleName) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Имя пользователя, пароль и роль обязательны.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (Password.Length < 8)
            {
                MessageBox.Show("Пароль должен состоять из 8 символов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            CloseAction?.Invoke(true);
        }

        private void Cancel()
        {
            CloseAction?.Invoke(false);
        }
    }
}
