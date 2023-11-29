using hrm_api.Controllers;
using HRM_Common.Models;
using HRM_Common.Models.Response;
using HRM_Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace HRM_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CandidateController : ControllerBase
    {
        private readonly ICandidateService candidateService;
        private readonly ILogger<CandidateController> logger;

        public CandidateController(ICandidateService _candidateService, ILogger<CandidateController> _logger)
        {
            candidateService = _candidateService;
            logger = _logger;
        }

        [HttpGet]
        [Route("get-all-candidates")]
        public async Task<List<Candidate>> GetAllCandidates(string? keyword)
        {
            var data = await candidateService.GetAllCandidates(keyword);
            return data;
        }
    }
}
