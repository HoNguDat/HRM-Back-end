using HRM_Common.Models.Response;
using HRM_Common.Models;
using HRM_Service.Services;
using Microsoft.AspNetCore.Mvc;
using HRM_Common.Paged;
using HRM_Common.ReqModules;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace HRM_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckInRecordController : ControllerBase
    {
        private readonly ICheckInRecordService _checkInRecordService;

        public CheckInRecordController(ICheckInRecordService checkInRecordService)
        {
            _checkInRecordService = checkInRecordService;
        }
        [HttpGet]
        [Route("get-all-paging-check-in")]
        public async Task<IActionResult> GetAllPaging([FromQuery] GetApplicationUserModule req)
        {
            var data = await _checkInRecordService.GetAllPaging(req);
            var result = new PagedResult<CheckInRecordRes>
            {
                Results = data.Results.Select(p => new CheckInRecordRes(p)).ToList(),
                Total = data.Total
            };
            return Ok(new { Message = "Get data success!", Data = result });
        }

        [HttpGet]
        [Route("get-check-in-byUserid")]
        public async Task<IActionResult> GetCheckInRecordByIdUser([FromQuery] GetApplicationUserModule req)
        {
            var data = await _checkInRecordService.GetCheckInRecordByIdUser(req);
            var result = new PagedResult<CheckInRecordRes>
            {
                Results = data.Results.Select(p => new CheckInRecordRes(p)).ToList(),
                Total = data.Total
            };
            return Ok(new { Message = "Get data success!", Data = result });
        }

        [HttpGet]
        [Route("get-check-in-byId/{id?}")]
        public async Task<IActionResult> GetCheckInRecordById(Guid id)

        {
            try
            {
                var data = await _checkInRecordService.GetCheckInRecordById(id);
                var result = new CheckInRecordRes(data);
                return Ok(new { Message = "Get data success!", Data = result });
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message.ToString() });
            }
        }


        [HttpGet]
        [Route("get-check-in")]
        public async Task<IActionResult> GetCheckInRecordByIdUser([FromQuery]DateTime? date,[FromQuery] string employeeId)

        {
            try
            {
                var data = await _checkInRecordService.GetCheckInRecordByDate(date, employeeId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message.ToString() });
            }
           
        }

        [HttpPost]
        [Route("check-in")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInRecord checkInRecord)
        {
            try
            {
                var data = await _checkInRecordService.CheckIn(checkInRecord);
                return Ok(new { Data = data, Message = "Check-in success", Status = "200", Description = "Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message.ToString(), Status = "400", Description = "Bad Request" });
            }
        }

        [HttpPost]
        [Route("check-out")]
        public async Task<IActionResult> CheckOut([FromBody] CheckInRecord checkInRecord)
        {
            try
            {
                var data = await _checkInRecordService.CheckOut(checkInRecord);
                return Ok(new { Data = data, Message = "Check-out success", Status = "200", Description = "Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message.ToString(), Status = "400", Description = "Bad Request" });
            }
        }

        [HttpPost]
        [Route("go-out")]
        public async Task<IActionResult> GoOut([FromBody] CheckInRecord checkInRecord)
        {
            try
            {
                var data = await _checkInRecordService.GoOut(checkInRecord);
                return Ok(new { Data = data, Message = "Go-out success", Status = "200", Description = "Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message.ToString(), Status = "400", Description = "Bad Request" });
            }
        }

        [HttpPost]
        [Route("go-in")]
        public async Task<IActionResult> Goin([FromBody] CheckInRecord checkInRecord)
        {
            try
            {
                var data = await _checkInRecordService.GoIn(checkInRecord);
                return Ok(new { Data = data, Message = "Go-in success", Status = "200", Description = "Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message.ToString(), Status = "400", Description = "Bad Request" });
            }
        }

        [HttpDelete]
        [Route("delete-check-in/{id}")]
        public async Task<IActionResult> DeleteCheckInRecord(Guid id)
        {
            var existingCheckInRecord = await _checkInRecordService.GetCheckInRecordById(id);
            if (existingCheckInRecord == null)
            {
                return BadRequest();
            }
            _checkInRecordService.DeleteCheckInRecord(id);
            return Ok();
        }
    }
}
