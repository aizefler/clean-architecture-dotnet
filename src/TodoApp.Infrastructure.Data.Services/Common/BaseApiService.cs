namespace TodoApp.Infrastructure.Data.Services.Common
{
    public abstract class BaseApiService
    {
        protected readonly HttpClient _httpClient;

        protected BaseApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
    }
}
