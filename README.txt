MOVEit Transfer
The repository contains a solution with three projects:

Infrastructure Project: This is the main project, encompassing the core functionality for establishing a connection and communication with the MOVEit API.

ConsoleClient: This project can be configured to monitor a local folder and manage file uploads and deletions in MOVEit. Upon starting the application, the user is prompted to enter their MOVEit credentials (username and password) for login. After a successful login, the user is asked to specify a local folder path. The application then begins monitoring this folder. Any file added to the local folder (including copy and paste actions) will be uploaded to MOVEit. Users can view these files in their MOVEit home folder. Similarly, if a file is deleted from the local folder, the application will send a request to delete the corresponding file from MOVEit.

WpfClient: This project offers the same functionality as the ConsoleClient but with a user-friendly graphical interface (UI), making it easier to set up and use. It also displays the local folder contents within the UI.

No external settings are required for this solution to work, other than valid MOVEit credentials (username and password) and an existing local directory.