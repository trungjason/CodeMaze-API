using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompaniesController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public CompaniesController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        public IActionResult GetCompanies() 
        {
            var companies = _serviceManager.CompanyService.GetAllCompanies(trackChanges: false);

            return Ok(companies);
        }

        [HttpGet("{id:guid}", Name = "CompanyById")]
        public IActionResult GetCompany(Guid id)
        {
            var company = _serviceManager.CompanyService.GetCompany(id, trackChanges: false);

            return Ok(company);
        }

        [HttpPost]
        public IActionResult CreateCompany([FromBody] CreateCompanyDTO company)
        {
            if (company is null)
                return BadRequest("CreateCompanyDTO object is null");

            var createdCompany = _serviceManager.CompanyService.CreateCompany(company);

            return CreatedAtAction("CompanyById", new { id = createdCompany.Id}, createdCompany);
        }
    }
}
