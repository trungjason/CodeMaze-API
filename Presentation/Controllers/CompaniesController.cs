using Microsoft.AspNetCore.Mvc;
using Presentation.ModelBinders;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompaniesController : ControllerBase
    {
        #region Constructor
        private readonly IServiceManager _serviceManager;

        public CompaniesController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        #endregion

        #region Get All
        [HttpGet]
        public IActionResult GetCompanies()
        {
            var companies = _serviceManager.CompanyService.GetAllCompanies(trackChanges: false);

            return Ok(companies);
        }

        [HttpGet("collection/({ids})", Name = "CompanyCollection")]
        public IActionResult GetCompanyCollection(
            [ModelBinder(BinderType = typeof(ArrayModelBinder))]
            IEnumerable<Guid> ids)
        {
            var companies = _serviceManager.CompanyService.GetByIds(ids, trackChanges: false);

            return Ok(companies);
        }
        #endregion

        #region Get By
        [HttpGet("{id:guid}", Name = "CompanyById")]
        public IActionResult GetCompany(Guid id)
        {
            var company = _serviceManager.CompanyService.GetCompany(id, trackChanges: false);

            return Ok(company);
        }
        #endregion

        #region Create
        [HttpPost]
        public IActionResult CreateCompany([FromBody] CreateCompanyDTO company)
        {
            if (company is null)
                return BadRequest("CreateCompanyDTO object is null");

            var createdCompany = _serviceManager.CompanyService.CreateCompany(company);

            return CreatedAtRoute("CompanyById", new { id = createdCompany.Id }, createdCompany);
        }

        [HttpPost("collection")]
        public IActionResult CreateCompayCollection([FromBody] IEnumerable<CreateCompanyDTO> companyCollection)
        {
            var result = _serviceManager.CompanyService.CreateCompanyCollection(companyCollection);

            return CreatedAtRoute("CompanyCollection", new { result.ids }, result.companies);
        }
        #endregion

        #region Update
        #endregion

        #region Delete
        #endregion
    }
}
