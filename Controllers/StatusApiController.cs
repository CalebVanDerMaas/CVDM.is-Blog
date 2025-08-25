using System.Threading.Tasks;
using CVDMBlog.Data.Repository;
using CVDMBlog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CVDMBlog.Controllers
{
    [Authorize(AuthenticationSchemes = "ApiKey")]
    [Route("api/status")]
    [ApiController]
    public class StatusApiController : ControllerBase
    {
        private readonly IRepository _repo;

        public StatusApiController(IRepository repo)
        {
            _repo = repo;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "ApiKey")]
        public async Task<IActionResult> UpdateStatus([FromBody] StatusUpdateModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newStatus = new Status
            {
                Content = model.Content,
                IsCurrent = true
            };

            _repo.AddStatus(newStatus);
            await _repo.SaveChangesAsync();

            return Ok(new { message = "Status updated successfully" });
        }
    }
}