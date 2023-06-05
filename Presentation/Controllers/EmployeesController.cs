using Entities.LinkModels;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using System.Text.Json;

namespace Presentation.Controllers
{
    [Route("/api/companies/{companyId}/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        #region Constructor
        private readonly IServiceManager _serviceManager;

        public EmployeesController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        #endregion

        #region Get All
        [HttpGet]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        public async Task<IActionResult> GetEmployeesForCompany(
            Guid companyId, 
            [FromQuery] 
            EmployeeParameters employeeParameters
            )
        {
            var linkParams = new LinkParameters(employeeParameters, HttpContext);

            //var pagedResult = await _serviceManager.EmployeeService.GetEmployeesAsync(companyId,
            //                            linkParams, trackChanges: false);

            var result = await _serviceManager.EmployeeService.GetEmployeesAsync(companyId,
                                    linkParams, trackChanges: false);


            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(result.metaData));

            return result.linkResponse.HasLinks ?   Ok(result.linkResponse.LinkedEntities) :
                                                    Ok(result.linkResponse.ShapedEntities);

        }
        #endregion

        #region Get By
        [HttpGet("{id:guid}", Name = "GetEmployeeForCompany")]
        public async Task<IActionResult> GetEmployee(Guid companyId, Guid id)
        {
            var employee = await _serviceManager.EmployeeService.GetEmployeeAsync(companyId, id, trackChanges: false);

            return Ok(employee);
        }
        #endregion

        #region Create
        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateEmployee(Guid companyId, [FromBody] CreateEmployeeDTO createEmployeeDTO)
        {
            var employeeToReturn = await _serviceManager.EmployeeService.CreateEmployeeForCompanyAsync(
                companyId, 
                createEmployeeDTO,
                trackChanges: false
            );

            return CreatedAtRoute("GetEmployeeForCompany", new
            {
                companyId,
                id = employeeToReturn.Id
            },
            employeeToReturn);

        }
        #endregion

        #region Update
        [HttpPut("{id:guid}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody] UpdateEmployeeDTO updateEmployeeDTO)
        {
            await _serviceManager.EmployeeService.UpdateEmployeeForCompanyAsync(
                companyId,
                id,
                updateEmployeeDTO,
                companyTrackChanges: false,
                employeeTrackChanges: true
            );

            return NoContent();
        }

        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(
            Guid companyId,
            Guid id,
            [FromBody] JsonPatchDocument<UpdateEmployeeDTO> patchDoc
        )
        {
            if (patchDoc is null)
                return BadRequest("patchDoc object sent from client is null.");

            var result = await _serviceManager.EmployeeService.GetEmployeeForPatchAsync(
                companyId, 
                id,
                compTrackChanges: false,
                empTrackChanges: true
            );
            
            // Validate JSON body 
            patchDoc.ApplyTo(result.employeeToPatch, ModelState);

            // This function will prevent employyToPatch contain age below 18
            // This will trigger validation and every error will make ModelState invalid
            TryValidateModel(result.employeeToPatch);

            await _serviceManager.EmployeeService.SaveChangesForPatchAsync(
                result.employeeToPatch,
                result.employeeEntity
            );

            return NoContent();
        }
        #endregion

        #region Delete
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteEmployee(Guid companyId, Guid id)
        {
            await _serviceManager.EmployeeService.DeleteEmployeeForCompanyAsync(companyId, id, trackChanges: false);

            return NoContent();
        }
        #endregion
    }
}
