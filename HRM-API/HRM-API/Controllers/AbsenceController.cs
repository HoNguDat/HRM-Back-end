using HRM_Common.Models;
using HRM_Common.Models.Response;
using HRM_Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace hrm_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AbsenceController : ControllerBase
    {
        private readonly IAbsenceService absenceService;
        private readonly ILogger<AbsenceController> logger;

        public AbsenceController(IAbsenceService _absenceService, ILogger<AbsenceController> _logger)
        {
            absenceService = _absenceService;
            logger = _logger;
        }

        [HttpGet]
        [Route("get-all-absences")]
        public async Task<List<AbsenceRes>> GetAllAbsences(string? keyword)
        {
            var data = await absenceService.GetAllAbsences(keyword);
            var result = data.Select(p => new AbsenceRes(p)).ToList();
            return result;
        }

        [HttpGet]
        [Route("get-all-absence-by-id")]
        public async Task<List<Absence>> GetAllAbsenceById(int id)
        {
            var result = await absenceService.GetAllAbsenceById(id);
            return result;
        }

        [HttpPost]
        [Route("booking-absence")]
        public async Task<IActionResult> BookingAbsence(Absence absence)
        {
            try
            {
                if(absence.Type == 1)
                {
                    if (absenceService.IsBookingDateValid(absence.FromDateSingle))
                    {
                        var data = await absenceService.BookingAbsence(absence);
                        var result = new AbsenceRes(data);
                        return Ok(new { Message = " Booking success !", Data = result });

                    }
                    return BadRequest("Booking failed, start date must be greater than current date !");
                }
                else
                {
                    if (absenceService.IsBookingDateValid(absence.FromDateMulti))
                    {
                        var data = await absenceService.BookingAbsence(absence);
                        var result = new AbsenceRes(data);
                        return Ok(new { Message = " Booking success !", Data = result });

                    }
                    return BadRequest("Booking failed, start date must be greater than current date !");
                }              
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [HttpDelete]
        [Route("remove-absence/{id}")]
        public async Task<IActionResult> RemoveAbsence(int id)
        {
            try
            {
                absenceService.RemoveAbsence(id);
                return Ok(new { Message = " Remove absnece success !" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
