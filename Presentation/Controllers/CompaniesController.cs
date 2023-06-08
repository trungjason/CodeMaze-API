using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Presentation.ModelBinders;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using System.Text.Json;

namespace Presentation.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    // Check Service config to know why 120SecondsDuration
    // This will apply to all action except GetCompanies because we specify an 
    // another Cache Attribute there => it will override this setting
    [ResponseCache(CacheProfileName = "120SecondsDuration")]
    [Authorize]
    [ApiExplorerSettings(GroupName = "v1")]
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
        /// <summary>
        /// Gets the list of all companies
        /// </summary>
        /// <returns>The companies list</returns>
        [HttpGet]
        // HttpHead return no response in the body (ASP.NET Core) will automatic handle that for us
        // This method is used to  obtain information about validity, accessibility,
        //and recent modifications of the resource. The response must be exactly like GET Method
        // but without body response
        [HttpHead]
        // This will add Header 'Cache-Control' to cache at client
        [ResponseCache(Duration = 60)]
        
        public async Task<IActionResult> GetCompanies([FromQuery] CompanyParameters companyParameters)
        {
            var companiesWithMetaData = await _serviceManager.CompanyService.GetAllCompaniesAsync(companyParameters, trackChanges: false);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(companiesWithMetaData.metaData));

            return Ok(companiesWithMetaData.companies);
        }

        [HttpGet("collection/({ids})", Name = "CompanyCollection")]
        public async Task<IActionResult> GetCompanyCollection(
            [ModelBinder(BinderType = typeof(ArrayModelBinder))]
            IEnumerable<Guid> ids)
        {
            var companies = await _serviceManager.CompanyService.GetByIdsAsync(ids, trackChanges: false);

            return Ok(companies);
        }
        #endregion

        #region Get By
        [HttpGet("{id:guid}", Name = "CompanyById")]
        public async Task<IActionResult> GetCompany(Guid id)
        {
            var company = await _serviceManager.CompanyService.GetCompanyAsync(id, trackChanges: false);

            return Ok(company);
        }
        #endregion

        #region Create
        /// <summary>
        /// Creates a newly created company
        /// </summary>
        /// <param name="company"></param>
        /// <returns>A newly created company</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>
        /// <response code="422">If the model is invalid</response>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyDTO createCompanyDTO)
        {
            var createdCompany = await _serviceManager.CompanyService.CreateCompanyAsync(createCompanyDTO);

            return CreatedAtRoute("CompanyById", new { id = createdCompany.Id }, createdCompany);
        }

        [HttpPost("collection")]
        public async Task<IActionResult> CreateCompayCollection([FromBody] IEnumerable<CreateCompanyDTO> companyCollection)
        {
            var result = await _serviceManager.CompanyService.CreateCompanyCollectionAsync(companyCollection);

            return CreatedAtRoute("CompanyCollection", new { result.ids }, result.companies);
        }
        #endregion

        #region Update
        [HttpPut("{id:guid}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] UpdateCompanyDTO updateCompanyDTO)
        {
            await _serviceManager.CompanyService.UpdateCompanyAsync(id, updateCompanyDTO, trackChanges: true);

            return NoContent();
        }
        #endregion

        #region Delete
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            await _serviceManager.CompanyService.DeleteCompanyAsync(id, false);

            return NoContent();
        }
        #endregion

        #region Option
        // This Action will be used be Client to check which method 
        // that our API route support
        [HttpOptions]
        public IActionResult GetCompaniesOptions()
        {
            Response.Headers.Add("Allow", "GET, OPTIONS, POST, DELETE");

            return Ok();
        }
        #endregion
    }
}
