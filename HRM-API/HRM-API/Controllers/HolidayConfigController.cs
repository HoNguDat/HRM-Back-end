using HRM_Common.Models;
using HRM_Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace HRM_API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class HolidayConfigController : ControllerBase
    {
        private readonly IHolidayConfigService _configService;

        public HolidayConfigController(IHolidayConfigService configService)
        {
            _configService = configService;
        }

        [HttpGet]
        [Route("get-all-holiday")]
        public async Task<List<HolidayConfig>> GetAllHolidayConfig()
        {
            var data = await _configService.GetAllHolidayConfig();
            return data;
        }

        [HttpPost]
        [Route("add-holiday-config")]
        public async Task<IActionResult> AddHolidayConfig([FromBody] HolidayConfig holidayConfig)
        {
            var data = await _configService.AddHolidayConfig(holidayConfig);
            return Ok(data);
        }
    }
}
