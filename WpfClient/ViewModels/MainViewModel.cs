using Infrastructure.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace WpfClient.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _statusLabel;
        private string _directory;
        private bool _isChooseButtonEnable;
        private FileSystemWatcher _systemWatcher;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            MoveItTransferService = new MoveItTransferService();
            Files = new ObservableCollection<Infrastructure.Models.File>();
            StatusLabel = "Enter credentials";
        }

        public ObservableCollection<Infrastructure.Models.File> Files { get; set; }

        public MoveItTransferService MoveItTransferService { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Directory
        {
            get { return _directory; }
            set
            {
                _directory = value;
                OnPropertyChanged(nameof(Directory));
            }
        }

        public string StatusLabel
        {
            get { return _statusLabel; }
            set
            {
                _statusLabel = value;
                OnPropertyChanged(nameof(StatusLabel));
            }
        }

        public bool IsChooseButtonEnable
        {
            get { return _isChooseButtonEnable; }
            set
            {
                _isChooseButtonEnable = value;
                OnPropertyChanged(nameof(IsChooseButtonEnable));
            }
        }

        public void SetupSystemWatcher(string directory)
        {
            _systemWatcher = new FileSystemWatcher(directory);

            _systemWatcher.Created += async (sender, eventArgs) => await OnCreated(sender, eventArgs);
            _systemWatcher.Deleted += async (sender, eventArgs) => await OnDelete(sender, eventArgs);
            _systemWatcher.EnableRaisingEvents = true;
        }

        public void UpdateFiles()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Files.Clear();

                List<string> files = System.IO.Directory.GetFileSystemEntries(_directory).ToList();
                foreach (string file in files)
                {
                    Files.Add(new Infrastructure.Models.File
                    {
                        Name = file,
                        Path = _directory,
                    });
                }
            });
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task OnCreated(object source, FileSystemEventArgs eventArgs)
        {
            try
            {
                using (var fileStream = File.OpenRead($"{eventArgs.FullPath}"))
                {
                    await MoveItTransferService.UploadFileAsync(fileStream, eventArgs.Name);
                    StatusLabel = $"File uploaded successfully to MOVEit!";
                    UpdateFiles();
                }
            }
            catch (Exception ex)
            {
                StatusLabel = ex.Message;
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task OnDelete(object source, FileSystemEventArgs eventArgs)
        {
            try
            {
                await MoveItTransferService.DeleteFileAsync(eventArgs.Name);
                StatusLabel = $"File deleted successfully from MOVEit!";
                UpdateFiles();
            }
            catch (Exception ex)
            {
                StatusLabel = ex.Message;
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
