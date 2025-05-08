using Application.ViewModel;
using Core.Entity;
using Core.Entity.Base;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace Application.Service.Base
{
    public abstract class BaseService<T> where T : BaseEntity
    {
        protected readonly HttpClient _httpClient;
        protected readonly string _baseUrl;

        protected BaseService(HttpClient httpClient, string baseUrl)
        {
            _httpClient = httpClient;
            _baseUrl = baseUrl;
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/contacts/{id}");

            var content = await response.Content.ReadAsStringAsync();

            var wrapper = JsonSerializer.Deserialize<ApiResponse<T>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return wrapper?.Data ?? throw new Exception("Contato não encontrado ou resposta inválida.");

        }

        public virtual async Task<IList<T>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/contacts");

            var content = await response.Content.ReadAsStringAsync();


            var wrapper = JsonSerializer.Deserialize<ApiResponse<IList<T>>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return wrapper?.Data ?? new List<T>();

        }


        public virtual async Task CreateAsync(T entity)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/contacts", entity);
            response.EnsureSuccessStatusCode();
        }

        public virtual async Task EditAsync(T entity)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/contacts/{entity.Id}", entity);
            response.EnsureSuccessStatusCode();
        }

        public virtual async Task DeleteAsync(T entity)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/contacts/{entity.Id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
