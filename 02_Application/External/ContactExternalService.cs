using Application.Service.Base;
using Application.Service.Interface;
using Microsoft.Extensions.Options;
using Application.ViewModel;
using Core.Entity;
using System.Net.Http.Json;

namespace Application.External
{
    public class ContactService : BaseService<Contact>, IContactService
    {
        private readonly HttpClient _httpClient;
        private readonly string _contactsApiBaseUrl;
        private readonly string _dddApiBaseUrl;

        public ContactService(HttpClient httpClient, IOptions<ApiSettings> options) : base(httpClient, options.Value.ContactStorageUrl)
        {
            _httpClient = httpClient;
            _contactsApiBaseUrl = options.Value.ContactStorageUrl;
        }

        public async Task<IList<Contact>> GetAllByDddAsync(int dddId)
        {
            // Verifica se o DDD existe via chamada externa
            var ddd = await _httpClient.GetFromJsonAsync<object>($"{_contactsApiBaseUrl}/api/v1/contacts/ddd-code/{dddId}");

            if (ddd is null)
                throw new ArgumentException("Invalid Direct Distance Dialing Id");

            // Busca contatos via chamada externa
            var contacts = await _httpClient.GetFromJsonAsync<IList<Contact>>($"{_contactsApiBaseUrl}/by-ddd/{dddId}");

            return contacts ?? new List<Contact>();
        }
    }
}
