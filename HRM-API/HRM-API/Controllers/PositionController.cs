using HRM_Common.Models;
using HRM_Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRM_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "Admin")]
    public class PositionController : ControllerBase
    {
        private readonly IPositionService positionService;
        public PositionController(IPositionService positionService)
        {
            this.positionService = positionService;
        }
        [HttpGet]
        [Route("get-all-positions")]
        public async Task<List<Position>> GetPositions()
        {
            var data = await positionService.GetAllPositions();
            return data;
        }

        [HttpPost]
        [Route("add-position")]
        public async Task<IActionResult> AddPosition([FromBody] Position position)
        {
            var existingPosition = await positionService.GetPositionByName(position.Name);
            if (existingPosition != null)
            {
                return BadRequest();
            }

            var data = await positionService.AddPosition(position);
            return Ok(data);
        }

        [HttpDelete]
        [Route("delete-position/{id}")]
        public async Task<IActionResult> DeletePosition(Guid id)
        {
            var existingPosition = await positionService.GetPositionById(id);
            if (existingPosition == null)
            {
                return BadRequest();
            }
            positionService.DeletePosition(id);
            return Ok();
        }

        [HttpPut]
        [Route("update-position/{id}")]
        public async Task<IActionResult> UpdatePosition(Guid id, [FromBody] Position position)
        {
            var data = await positionService.UpdatePosition(id, position);
            return Ok(data);
        }
    }
}
