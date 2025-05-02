using Core.Entity.Base;
using System.Net.Http.Json;

namespace Application.Service.Base
{
    public abstract class BaseService<T> where T : BaseEntity
    {
        protected readonly HttpClient _httpClient;
        protected readonly string _baseUrl;

        protected BaseService(HttpClient httpClient, string baseUrl)
        {
            _httpClient = httpClient;
            _baseUrl = baseUrl; // Ex: "https://api.exemplo.com/entidade"
        }


        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<T>($"{_baseUrl}/{id}");
        }

        public virtual async Task<IList<T>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<T>>(_baseUrl);
        }

        public virtual async Task CreateAsync(T entity)
        {
            var response = await _httpClient.PostAsJsonAsync(_baseUrl, entity);
            response.EnsureSuccessStatusCode();
        }
        public virtual async Task EditAsync(T entity)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/{entity.Id}", entity);
            response.EnsureSuccessStatusCode();
        }

        public virtual async Task DeleteAsync(T entity)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/{entity.Id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
