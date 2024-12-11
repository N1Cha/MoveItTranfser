using Infrastructure.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace Infrastructure.Services
{
    public class MoveItTransferService : IMoveItTransferService
    {
        private readonly string _baseUrl = "https://testserver.moveitcloud.com/api/v1/";
        private static readonly HttpClient _httpClient = new HttpClient();

        private User _user;
        private TokenJwt _token;
        private DateTime _tokenExpiration;

        public MoveItTransferService()
        {
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            _token = new TokenJwt();
            _tokenExpiration = DateTime.MinValue;
        }

        public async Task LogInAsync(string username, string password)
        {
            _token = await GetJwtTokenAsync(username, password);
            _tokenExpiration = DateTime.UtcNow.AddSeconds(_token.ExpiresIn);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token.AccessToken);
            _user = new User() { Username = username, Password = password };
        }

        public async Task UploadFileAsync(Stream fileStream, string fileName)
        {
            await RefreshTokenAsync();

            string homeFolderId = await GetUserHomeFolderId();
            string url = $"folders/{homeFolderId}/files";

            using (MultipartFormDataContent content = new MultipartFormDataContent())
            {
                using (StreamContent fileContent = new StreamContent(fileStream))
                {
                    content.Add(fileContent, "file", fileName);

                    HttpResponseMessage response = await _httpClient.PostAsync(url, content);
                    response.EnsureSuccessStatusCode();
                }
            }
        }

        public async Task DeleteFileAsync(string fileName)
        {
            await RefreshTokenAsync();

            List<Models.File> files = await GetUserFilesAsync();
            string fileId = files?.First(x => string.Compare(x.Name, fileName, true) == 0).Id;

            string url = $"files/{fileId}";

            HttpResponseMessage response = await _httpClient.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
        }

        public async Task RefreshTokenAsync()
        {
            if (DateTime.UtcNow >= _tokenExpiration)
            {
                _token = await GetJwtTokenAsync(_user.Username, _user.Password);
                _tokenExpiration = DateTime.UtcNow.AddSeconds(_token.ExpiresIn);
            }
        }

        private async Task<List<Models.File>> GetUserFilesAsync()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("files");
            response.EnsureSuccessStatusCode();

            string responseData = await response.Content.ReadAsStringAsync();
            return JObject.Parse(responseData)["items"].ToObject<List<Models.File>>();
        }

        private async Task<string> GetUserHomeFolderId()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("users/self");
            response.EnsureSuccessStatusCode();

            JObject jsonObject = JObject.Parse(await response.Content.ReadAsStringAsync());
            string homeFolderId = jsonObject["homeFolderID"].ToString();

            return homeFolderId;
        }

        private async Task<TokenJwt> GetJwtTokenAsync(string username, string password)
        {
            Dictionary<string, string> authPayload = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "username", username },
                { "password", password }
            };

            HttpResponseMessage response = await _httpClient.PostAsync("token", new FormUrlEncodedContent(authPayload));
            response.EnsureSuccessStatusCode();

            string responseData = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TokenJwt>(responseData);
        }
    }
}
