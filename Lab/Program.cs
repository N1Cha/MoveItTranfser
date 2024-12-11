using Infrastructure.Services;

namespace ConsoleClient
{
    public class Program
    {
        private static readonly string _username = "interview.nikola.chalakov";
        private static readonly string _password = "DQxP3h?E";
        private static readonly string _folderPath = "C:\\Users\\n1k1\\Music";

        private static MoveItTransferService _moveItService;

        public static async Task Main()
        {
            _moveItService = new MoveItTransferService();
            LoginStatusCode loginCode = LoginStatusCode.Unauthorized;
            DirectoryStatusCode folderCode = DirectoryStatusCode.NotFound;

            do
            {
                try
                {
                    await HandleLogIn();
                    loginCode = LoginStatusCode.Authorized;
                    ColoredConsole.WriteLine($"Login Successful!", ConsoleColor.Black, ConsoleColor.Green);
                }
                catch
                {
                    loginCode = LoginStatusCode.Unauthorized;
                    ColoredConsole.WriteErrorLine($"Wrong Username or Password!", ConsoleColor.Black, ConsoleColor.Red);
                    ColoredConsole.WriteLine("Please try again!");
                }
            }
            while (loginCode != LoginStatusCode.Authorized);

            do
            {
                try
                {
                    SetupPathDirectory();
                    folderCode = DirectoryStatusCode.Exists;
                }
                catch (DirectoryNotFoundException ex)
                {
                    ColoredConsole.WriteErrorLine(ex.Message);
                    ColoredConsole.WriteLine("Please try again!");
                    folderCode = DirectoryStatusCode.NotFound;
                }
                catch (Exception ex)
                {
                    ColoredConsole.WriteErrorLine($"Error with Directory {ex.Message}");
                    folderCode = DirectoryStatusCode.Error;
                }
            }
            while (folderCode == DirectoryStatusCode.NotFound);

            ColoredConsole.WriteLine(Environment.NewLine + "=========== Press any key to EXIT! ===========" + Environment.NewLine, ConsoleColor.Black, ConsoleColor.Red);
            Console.ReadLine();
        }

        private static async Task OnCreated(object source, FileSystemEventArgs eventArgs)
        {
            ColoredConsole.WriteLine($"File add to directory: {eventArgs.FullPath}");
            try
            {
                using (FileStream fileStream = File.OpenRead($"{eventArgs.FullPath}"))
                {
                    await _moveItService.UploadFileAsync(fileStream, eventArgs.Name);
                    ColoredConsole.WriteLine($"===== File uploaded successfully to MOVEit!", ConsoleColor.Black, ConsoleColor.Green);
                }
            }
            catch (Exception ex)
            {
                ColoredConsole.WriteErrorLine(ex.Message);
            }
        }

        private static async Task OnDelete(object source, FileSystemEventArgs eventArgs)
        {
            ColoredConsole.WriteLine($"File Deleted from directory: {eventArgs.FullPath}");

            try
            {
                await _moveItService.DeleteFileAsync(eventArgs.Name);
                ColoredConsole.WriteLine($"===== File deleted successfully from MOVEit!", ConsoleColor.Black, ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                ColoredConsole.WriteErrorLine(ex.Message);
            }

        }

        private static async Task HandleLogIn()
        {
            ColoredConsole.Write("Please Enter username:");
            string username = Console.ReadLine();

            ColoredConsole.Write("Please Enter password:");
            string password = Console.ReadLine();

#if DEBUG 
            //This code is only added to speed up application development
            username = string.IsNullOrEmpty(username) ? _username : username;
            password = string.IsNullOrEmpty(password) ? _password : password;
#endif

            await _moveItService.LogInAsync(username, password);            
        }

        private static FileSystemWatcher SetupPathDirectory()
        {
            ColoredConsole.Write("Please enter a directory:");
            string folderPath = Console.ReadLine();

#if DEBUG
            //This code is only added to speed up application development
            folderPath = string.IsNullOrEmpty(folderPath) ? _folderPath : folderPath;
#endif

            if (!Directory.Exists(folderPath))
            {
                throw new DirectoryNotFoundException($"Directory does not exist: {folderPath}");
            }

            FileSystemWatcher systemWatcher = new FileSystemWatcher(folderPath);
            systemWatcher.Filter = "*.*";
            systemWatcher.Created += async (sender, eventArgs) => await OnCreated(sender, eventArgs);
            systemWatcher.Deleted += async (sender, eventArgs) => await OnDelete(sender, eventArgs);
            systemWatcher.EnableRaisingEvents = true;

            ColoredConsole.WriteLine($"Folder [ {folderPath} ] is ready and connected to MOVEit Transfer!", ConsoleColor.Black, ConsoleColor.Cyan);
            return systemWatcher;
        }
    }
}
