using Application.Service.Base;
using Application.ViewModel;
using Core.Entity;
using FluentAssertions;
using RichardSzalay.MockHttp;
using System.Net;
using System.Text.Json;

namespace UnitTest.Repository
{
    public class ContactRepositoryTests
    {
        private const string BaseUrl = "http://fake-api.com";


        private class ContactHttpService : BaseService<Contact>
        {
            public ContactHttpService(HttpClient client, string baseUrl) : base(client, baseUrl) { }
        }

        [Fact]
        public async Task GetByIdAsync_ValidResponse_ReturnsContact()
        {
            var contact = new Contact
            {
                Id = 1,
                Name = "Maria",
                Phone = "99999-0000",
                Email = "maria@mail.com",
                DddId = 11
            };

            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"{BaseUrl}/contacts/1")
                    .Respond("application/json", JsonSerializer.Serialize(new ApiResponse<Contact> { Data = contact }));

            var service = new ContactHttpService(mockHttp.ToHttpClient(), BaseUrl);

            var result = await service.GetByIdAsync(1);

            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("Maria");
        }

        [Fact]
        public async Task GetByIdAsync_InvalidJson_ThrowsException()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"{BaseUrl}/contacts/1")
                    .Respond("application/json", "{}"); // resposta inválida

            var service = new ContactHttpService(mockHttp.ToHttpClient(), BaseUrl);

            Func<Task> act = async () => await service.GetByIdAsync(1);
            await act.Should().ThrowAsync<Exception>().WithMessage("*resposta inválida*");
        }
        [Fact]
        public async Task GetAllAsync_ValidResponse_ReturnsContacts()
        {
            var contacts = new List<Contact>
        {
            new() { Id = 1, Name = "A", Phone = "1", Email = "a@mail.com", DddId = 11 },
            new() { Id = 2, Name = "B", Phone = "2", Email = "b@mail.com", DddId = 11 }
        };

            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"{BaseUrl}/contacts")
                    .Respond("application/json", JsonSerializer.Serialize(new ApiResponse<IList<Contact>> { Data = contacts }));

            var service = new ContactHttpService(mockHttp.ToHttpClient(), BaseUrl);

            var result = await service.GetAllAsync();

            result.Should().HaveCount(2);
            result[0].Name.Should().Be("A");
        }

        [Fact]
        public async Task GetAllAsync_NullData_ReturnsEmptyList()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"{BaseUrl}/contacts")
                    .Respond("application/json", "{}");

            var service = new ContactHttpService(mockHttp.ToHttpClient(), BaseUrl);

            var result = await service.GetAllAsync();

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_ValidContact_Succeeds()
        {
            var contact = new Contact
            {
                Name = "Carlos",
                Phone = "99999-9999",
                Email = "carlos@mail.com",
                DddId = 21
            };

            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"{BaseUrl}/contacts")
                    .Respond(HttpStatusCode.Created);

            var service = new ContactHttpService(mockHttp.ToHttpClient(), BaseUrl);

            var act = async () => await service.CreateAsync(contact);
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task EditAsync_RequestFails_ThrowsException()
        {
            var contact = new Contact
            {
                Id = 7,
                Name = "Edit Fail",
                Phone = "00000-0000",
                Email = "fail@mail.com",
                DddId = 99
            };

            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"{BaseUrl}/contacts/7")
                    .Respond(HttpStatusCode.BadRequest);

            var service = new ContactHttpService(mockHttp.ToHttpClient(), BaseUrl);

            Func<Task> act = async () => await service.EditAsync(contact);
            await act.Should().ThrowAsync<HttpRequestException>();
        }

        [Fact]
        public async Task DeleteAsync_SuccessfulDeletion_DoesNotThrow()
        {
            var contact = new Contact
            {
                Id = 3,
                Name = "Delete Me",
                Phone = "00000-1234",
                Email = "delete@mail.com",
                DddId = 11
            };

            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"{BaseUrl}/contacts/3")
                    .Respond(HttpStatusCode.NoContent);

            var service = new ContactHttpService(mockHttp.ToHttpClient(), BaseUrl);

            var act = async () => await service.DeleteAsync(contact);
            await act.Should().NotThrowAsync();
        }
    }
}
