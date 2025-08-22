using ASPIdentityApp.DTOs;
using ASPIdentityApp.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPIdentityApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // protect all endpoints by default
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _service;
        private readonly ILogger<UserProfileController> _logger;

        public UserProfileController(IUserProfileService service, ILogger<UserProfileController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _service.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserProfileDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserProfileDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound();
            return Ok(new { success = true });
        }
    }
}
