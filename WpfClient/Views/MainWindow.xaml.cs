using System.Windows;
using System.Windows.Controls;
using WpfClient.ViewModels;

namespace WpfClient
{
    public partial class MainWindow : Window
    {
        private MainViewModel _mainViewModel;

        public MainWindow()
        {
            InitializeComponent();
            _mainViewModel = new MainViewModel();
            DataContext = _mainViewModel;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string status = string.Empty;
            Task.Run(async () =>
            {
                try
                {
                    await _mainViewModel.MoveItTransferService.LogInAsync(_mainViewModel.Username, _mainViewModel.Password);
                    _mainViewModel.IsChooseButtonEnable = true;
                    status = "Login Successful!";
                }
                catch
                {
                    status = "Wrong Username or Password! Please try again!";
                }
            }).Wait();

            _mainViewModel.StatusLabel = status;
        }

        private void ChooseButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFolderDialog folderDialog = new Microsoft.Win32.OpenFolderDialog();
            folderDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

            bool? result = folderDialog.ShowDialog();
            if (result == true)
            {
                _mainViewModel.StatusLabel = $"Directory [ {folderDialog.FolderName} ] selected!";
                _mainViewModel.Directory = folderDialog.FolderName;
                _mainViewModel.SetupSystemWatcher(folderDialog.FolderName);

                _mainViewModel.UpdateFiles();
            }
        }
    }
}