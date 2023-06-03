using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Presentation.Controllers
{
    [Route("/api/companies/{companyId}/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public EmployeesController(IServiceManager serviceManager) 
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        public IActionResult GetAllEmployees(Guid companyId)
        {
            var employees = _serviceManager.EmployeeService.GetAllEmployees(companyId, trackChanges: false);

            return Ok(employees);
        }

        [HttpGet("{id:guid}", Name = "GetEmployeeForCompany")]
        public IActionResult GetEmployee(Guid companyId, Guid id)
        {
            var employee = _serviceManager.EmployeeService.GetEmployee(companyId, id, trackChanges: false);

            return Ok(employee);
        }

        [HttpPost]
        public IActionResult CreateEmployee(Guid companyId, [FromBody] CreateEmployeeDTO createEmployeeDTO)
        {
            if (createEmployeeDTO is null)
            {
                return BadRequest("EmployeeForCreationDto object is null");
            }
            var employeeToReturn = _serviceManager.EmployeeService.CreateEmployee(companyId, createEmployeeDTO, 
                trackChanges: false
            );
            return CreatedAtRoute("GetEmployeeForCompany", new
            {
                companyId, id = employeeToReturn.Id
            },
            employeeToReturn);

        }
    }
}
