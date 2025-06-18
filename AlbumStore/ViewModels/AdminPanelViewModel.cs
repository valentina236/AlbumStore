using System;
using System.Collections.ObjectModel;
using System.Linq;
using AlbumStore.Helpers;
using AlbumStore.Models;
using System.Windows.Input;
using System.Windows;
using AlbumStore.Services;
using Microsoft.Win32;
using System.IO;
using AlbumStore.Views;

namespace AlbumStore.ViewModels
{
    public class AdminPanelViewModel : BaseViewModel
    {
        private ObservableCollection<User> _users;
        private readonly DatabaseService _databaseService;
        private bool _isEditDialogOpen;
        private User _selectedUser;

        public ObservableCollection<User> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged(nameof(Users));
            }
        }

        public bool IsEditDialogOpen
        {
            get => _isEditDialogOpen;
            set
            {
                _isEditDialogOpen = value;
                OnPropertyChanged(nameof(IsEditDialogOpen));
            }
        }

        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
            }
        }

        public ICommand AddUserCommand { get; }
        public ICommand EditUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand ResetPasswordCommand { get; }
        public ICommand CreateBackupCommand { get; }
        public ICommand RestoreBackupCommand { get; }
        public ICommand SaveUserCommand { get; }
        public ICommand CancelEditCommand { get; }

        public AdminPanelViewModel()
        {
            _databaseService = new DatabaseService();

            LoadUsers();

            AddUserCommand = new RelayCommand(AddUser);
            EditUserCommand = new RelayCommand(EditUser);
            DeleteUserCommand = new RelayCommand(DeleteUser);
            ResetPasswordCommand = new RelayCommand(ResetPassword);
            CreateBackupCommand = new RelayCommand(CreateBackup);
            RestoreBackupCommand = new RelayCommand(RestoreBackup);
            SaveUserCommand = new RelayCommand(SaveUser);
            CancelEditCommand = new RelayCommand(CancelEdit);
        }

        private void LoadUsers()
        {
            try
            {
                var allUsers = _databaseService.GetUsers();
                Users = new ObservableCollection<User>(allUsers.Where(u => u.RoleName != "admin"));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки юзеров: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddUser(object parameter)
        {
            var addUserWindow = new AddUserWindow();
            if (addUserWindow.ShowDialog() == true)
            {
                var viewModel = addUserWindow.ViewModel;
                try
                {
                    var existingUser = Users.FirstOrDefault(u => u.UserName == viewModel.UserName);
                    if (existingUser != null)
                    {
                        MessageBox.Show("Пользователь с таким именем уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var newUser = new User
                    {
                        UserName = viewModel.UserName,
                        RoleName = viewModel.RoleName,
                        UserPassword = string.IsNullOrWhiteSpace(viewModel.Password)
                            ? BCrypt.Net.BCrypt.HashPassword("default123")
                            : BCrypt.Net.BCrypt.HashPassword(viewModel.Password)
                    };

                    _databaseService.AddUser(newUser);
                    LoadUsers();
                    LogAction($"Added user: {newUser.UserName}");
                    MessageBox.Show($"Пользователь {newUser.UserName} добавлен. Пароль: {(string.IsNullOrWhiteSpace(viewModel.Password) ? "default123" : "задан вручную")}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка добавления пользователя: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void EditUser(object parameter)
        {
            if (parameter is User user)
            {
                SelectedUser = new User
                {
                    IdUser = user.IdUser,
                    UserName = user.UserName,
                    RoleName = user.RoleName,
                    UserPassword = user.UserPassword
                };
                IsEditDialogOpen = true;
            }
        }

        private void DeleteUser(object parameter)
        {
            if (parameter is User user && MessageBox.Show($"Вы уверены, что хотите удалить пользователя '{user.UserName}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    _databaseService.DeleteUser(user.IdUser);
                    LogAction($"Deleted user: {user.UserName}");
                    MessageBox.Show($"Пользователь '{user.UserName}' успешно удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    Users.Remove(user);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении пользователя: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ResetPassword(object parameter)
        {
            if (parameter is User user)
            {
                try
                {
                    string tempPassword = Guid.NewGuid().ToString().Substring(0, 8);
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(tempPassword);
                    user.UserPassword = hashedPassword;
                    _databaseService.UpdateUser(user);
                    LogAction($"Reset password for user: {user.UserName}");
                    MessageBox.Show($"Пароль для {user.UserName} сброшен. Временный пароль: {tempPassword}", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сброса пароля: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveUser(object parameter)
        {
            if (SelectedUser == null)
            {
                MessageBox.Show("Пользователь не выбран.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(SelectedUser.UserName) || string.IsNullOrWhiteSpace(SelectedUser.RoleName))
            {
                MessageBox.Show("Имя пользователя и роль обязательны.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var existingUser = Users.FirstOrDefault(u => u.UserName == SelectedUser.UserName && u.IdUser != SelectedUser.IdUser);
                if (existingUser != null)
                {
                    MessageBox.Show("Пользователь с таким именем уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var userToUpdate = Users.FirstOrDefault(u => u.IdUser == SelectedUser.IdUser);
                if (userToUpdate != null)
                {
                    userToUpdate.UserName = SelectedUser.UserName;
                    userToUpdate.RoleName = SelectedUser.RoleName;
                    _databaseService.UpdateUser(userToUpdate);
                    LogAction($"Updated user: {SelectedUser.UserName}");
                    MessageBox.Show($"Пользователь {SelectedUser.UserName} обновлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                IsEditDialogOpen = false;
                SelectedUser = null;
                LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения пользователя: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelEdit(object parameter)
        {
            IsEditDialogOpen = false;
            SelectedUser = null;
        }

        private void CreateBackup(object parameter)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Backup Files (*.bak)|*.bak",
                Title = "Выберите место для сохранения резервной копии",
                DefaultExt = "bak"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    _databaseService.CreateBackupDB(saveFileDialog.FileName);
                    LogAction($"Created backup: {saveFileDialog.FileName}");
                    MessageBox.Show($"Резервная копия успешно создана: {saveFileDialog.FileName}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка создания резервной копии: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RestoreBackup(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Backup Files (*.bak)|*.bak",
                Title = "Выберите файл резервной копии для восстановления"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    _databaseService.RestoreBackupDB(openFileDialog.FileName);
                    LogAction($"Restored backup: {openFileDialog.FileName}");
                    MessageBox.Show($"Данные успешно восстановлены из: {openFileDialog.FileName}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadUsers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка восстановления данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LogAction(string action)
        {
            try
            {
                File.AppendAllText("admin_log.txt", $"{DateTime.Now}: {action}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging action: {ex.Message}");
            }
        }
    }
}
