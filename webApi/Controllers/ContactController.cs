using Application.External;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly ContactService _contactService;

        public ContactController(ContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpGet("ddd/{id}")]
        public async Task<IActionResult> GetByDdd(int id)
        {
            var result = await _contactService.GetAllByDddAsync(id);
            return Ok(result);
        }
    }
}
