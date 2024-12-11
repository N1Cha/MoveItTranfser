namespace Infrastructure.Services
{
    public interface IMoveItTransferService
    {
        Task LogInAsync(string username, string password);

        Task UploadFileAsync(Stream fileStream, string fileName);

        Task DeleteFileAsync(string fileName);
    }
}
