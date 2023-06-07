using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        #region Constructor
        private readonly IServiceManager _service;
        public TokenController(IServiceManager service) => _service = service;
        #endregion

        [HttpPost("refresh")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Refresh([FromBody] TokenDTO tokenDto)
        {
            var tokenDtoToReturn = await _service.AuthenticationService.RefreshToken(tokenDto);

            return Ok(tokenDtoToReturn);
        }
    }
}
