using HRM_Common.Models;
using HRM_Common.Models.Response;
using HRM_Common.Paged;
using HRM_Common.ReqModules;
using HRM_Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace HRM_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PayRollController : ControllerBase
    {
        private readonly IPayrollService _payrollService;
        public PayRollController(IPayrollService payrollService)
        {
            _payrollService = payrollService;
        }

        [HttpGet]
        [Route("get-all-paging-payroll")]
        public async Task<IActionResult> GetAllPaging([FromQuery] GetApplicationUserModule req)
        {
            var data = await _payrollService.GetAllPaging(req);
            var result = new PagedResult<PayRollRes>
            {
                Results = data.Results.Select(p => new PayRollRes(p)).ToList(),
                Total = data.Total
            };
            return Ok(new { Message = "Get data success!", Data = result });
        }

        [HttpPost]
        [Route("add-payroll")]
        public async Task<IActionResult> AddPayRoll([FromBody] Payroll payroll)
        {
            try
            {
                var data = await _payrollService.AddPayRoll(payroll);
                return Ok(new { Data = data, Message = "Add payroll success", Status = "200", Description = "Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message.ToString(), Status = "400", Description = "Bad Request" });
            }
        }
    }
}
