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

        public AbsenceController(IAbsenceService _absenceService)
        {
            absenceService = _absenceService;
        }

        [HttpGet]
        [Route("get-all-absences")]
        public async Task<List<AbsenceRes>> GetAllAbsences()
        {
            var data = await absenceService.GetAllAbsences();
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
        public async Task<AbsenceRes> BookingAbsence(Absence absence)
        {
            var data = await absenceService.BookingAbsence(absence);
            var result = new AbsenceRes(data);
            return result;
        }
    }
}
