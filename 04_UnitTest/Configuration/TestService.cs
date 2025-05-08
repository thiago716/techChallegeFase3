using Application.Service.Base;
using Core.Entity;

namespace UnitTest.Configuration
{
    public class TestService : BaseService<Contact>
    {
        public TestService(HttpClient client, string baseUrl) : base(client, baseUrl) { }
    }
}
