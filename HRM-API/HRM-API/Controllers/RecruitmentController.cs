using HRM_Common.Models.Response;
using HRM_Common.Models;
using HRM_Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRM_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class RecruitmentController : ControllerBase
    {
        private readonly IRecruitmentService recruitmentService;
        private readonly ApplicationDbContext _context;

        public RecruitmentController(IRecruitmentService _recruitmentService, ApplicationDbContext context)
        {
            recruitmentService = _recruitmentService;
            _context = context;
        }

        [HttpGet]
        [Route("get-all-recruitment")]
        public async Task<List<RecruitmentRes>> GetRecruitment(string? keyword)
        {
            var data = await recruitmentService.GetAllRecruitment(keyword);
            var result = data.Select(p => new RecruitmentRes(p)).ToList();
            return result;
        }

        [HttpPost]
        [Route("add-recruitment")]
        public async Task<IActionResult> AddRecruitment([FromBody] Recruitment recruitment)
        {
            var data = await recruitmentService.AddRecruitment(recruitment);
            var result = new RecruitmentRes(data);
            return Ok(result);
        }

        [HttpGet]
        [Route("get-recruitment/{id?}")]
        public async Task<RecruitmentRes> GetRecruitment(Guid id)
        {
            var data = await recruitmentService.GetRecruitmentById(id);
            var result = new RecruitmentRes(data);
            return result;
        }

        [HttpDelete]
        [Route("delete-recruitment/{id}")]
        public async Task<IActionResult> DeleteRecruitment(Guid id)
        {
            var existingRecruitment = await recruitmentService.GetRecruitmentById(id);
            if (existingRecruitment == null)
            {
                return BadRequest();
            }
            recruitmentService.DeleteRecruitment(id);
            return Ok();
        }

        [HttpPut]
        [Route("update-recruitment/{id}")]
        public async Task<IActionResult> UpdateRecruitment(Guid id, [FromBody] Recruitment recruitment)
        {
            var existingRecruitment = await _context.Recruitments.FindAsync(id);
            if (existingRecruitment == null)
            {
                return NotFound();
            }
            if (recruitment.Version != existingRecruitment.Version)
            {
                return Conflict();
            }
            var data = await recruitmentService.UpdateRecruitment(id, recruitment);
            return Ok(data);
        }
    }
}
