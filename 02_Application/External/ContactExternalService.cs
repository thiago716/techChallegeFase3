using Application.Service.Base;
using Application.Service.Interface;
using Microsoft.Extensions.Options;
using Application.ViewModel;
using Core.Entity;
using System.Net.Http.Json;
using System.Text.Json;

namespace Application.External
{
    public class ContactExternalService : BaseService<Contact>, IContactService
    {
        private readonly HttpClient _httpClient;
        private readonly string _contactsApiBaseUrl;

        public ContactExternalService(HttpClient httpClient, IOptions<ApiSettings> options)
     : base(httpClient, options.Value.ContactStorageUrl)
        {
            _httpClient = httpClient;
            _contactsApiBaseUrl = options.Value.ContactStorageUrl; 
        }

        public async Task<IList<ContactResult>> GetAllByDddAsync(int dddId)
        {
            var jsonString = await _httpClient.GetStringAsync($"{_contactsApiBaseUrl}/contacts/ddd-code/{dddId}");

            using var document = JsonDocument.Parse(jsonString);

            if (!document.RootElement.TryGetProperty("data", out var dataElement) || dataElement.ValueKind != JsonValueKind.Array)
                throw new ArgumentException("Invalid Direct Distance Dialing Id");

            var result = new List<ContactResult>();

            foreach (var item in dataElement.EnumerateArray())
            {
                var dto = new ContactResult
                {
                    Name = item.GetProperty("name").GetString() ?? string.Empty,
                    Phone = item.GetProperty("phone").GetString() ?? string.Empty,
                    Email = item.GetProperty("email").GetString() ?? string.Empty,
                    Ddd = item.GetProperty("dddId").GetInt32(),
                    Region = item.TryGetProperty("ddd", out var dddElement) && dddElement.TryGetProperty("region", out var regionProp)
                                ? regionProp.GetString() ?? string.Empty
                                : string.Empty
                };

                result.Add(dto);
            }

            return result;
        }
    }
}
