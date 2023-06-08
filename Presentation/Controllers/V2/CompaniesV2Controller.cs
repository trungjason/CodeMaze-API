using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;

namespace Presentation.Controllers.V2
{
    // We can also specify Deprecated API version
    [ApiVersion("2.0", Deprecated = true)]
    [ApiController]
    [Route("api/{v:apiversion}/companies")]
    [ApiExplorerSettings(GroupName = "v2")]
    public class CompaniesV2Controller : ControllerBase
    {
        private readonly IServiceManager _service;
        public CompaniesV2Controller(IServiceManager service) => _service = service;

        [HttpGet]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
        [HttpCacheValidation(MustRevalidate = false)]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await _service.CompanyService
                                .GetAllCompaniesAsync(null, trackChanges: false);

            var companiesV2 = companies.companies.Select(x => $"{x.Name} V2");

            return Ok(companiesV2);
        }
    }
}
