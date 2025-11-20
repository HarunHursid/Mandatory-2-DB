using DiscProfilesApi.DTOs;
using DiscProfilesApi.Interfaces;
using DiscProfilesApi.Models;
using DiscProfilesApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DiscProfilesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployee_PersonDomainInterface _employeeDomainService;
        private readonly IGenericService<employee,EmployeeDTO> _employeeService;

        public EmployeeController(IEmployee_PersonDomainInterface employeeDomainService, 
            IGenericService<employee, EmployeeDTO> employeeService )
        {
            _employeeDomainService = employeeDomainService;
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _employeeService.GetAllAsync();
            return Ok(employees);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEmployee_PersonRequestDto dto)
        {
            var created = await _employeeDomainService.CreateEmployeeWithPersonAsync(dto);
            return Ok(created);
        }


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] EmployeeDTO dto)
        {
            if (id != dto.id)
                return BadRequest("URL id does not match body id.");

            var updated = await _employeeService.UpdateAsync(id,dto);
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _employeeService.DeleteAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }

    }
}
