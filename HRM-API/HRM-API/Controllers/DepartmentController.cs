using HRM_Common.Models;
using HRM_Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRM_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
   [Authorize(Roles = "Admin")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService departmentService;

        public DepartmentController(IDepartmentService _departmentService)
        {
            departmentService = _departmentService;
        }

        [HttpGet]
        [Route("get-all-department")]
        public async Task<List<Department>> GetDepartments(string? keyword)
        {
            var data = await departmentService.GetAllDepartments(keyword);
            return data;
        }

        [HttpPost]
        [Route("add-department")]
        public async Task<IActionResult> AddDepartment([FromBody] Department departmentRequest)
        {
            var existingDepartment = await departmentService.GetDepartmentByName(departmentRequest.Name);
            if (existingDepartment != null)
            {
                return BadRequest();
            }

            var data = await departmentService.AddDepartment(departmentRequest);
            return Ok(data);
        }

        [HttpDelete]
        [Route("delete-department/{id}")]
        public async Task<IActionResult> DeleteDepartment(Guid id)
        {
            var existingDepartment = await departmentService.GetDepartmentById(id);
            if (existingDepartment == null)
            {
                return BadRequest();
            }
            departmentService.DeleteDepartment(id);
            return Ok();
        }

        [HttpPut]
        [Route("update-department/{id}")]
        public async Task<IActionResult> UpdateDepartment(Guid id, [FromBody] Department department)
        {
            var data = await departmentService.UpdateDepartment(id, department);
            return Ok(data);
        }
    }
}
