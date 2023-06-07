using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Presentation.ActionFilters;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        #region Constructor
        private readonly IServiceManager _serviceManager;
        
        public AuthenticationController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        #endregion

        #region Register
        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> RegisterUser(
            [FromBody] UserForRegistrationDTO userForRegistration)
        {
            var result = await _serviceManager.AuthenticationService
                .RegisterUser(userForRegistration);
                
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }

                return BadRequest(ModelState);
            }

            return StatusCode(201);
        }
        #endregion

        #region Authenticate
        [HttpPost("login")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Authenticate(
            [FromBody] UserForAuthenticationDTO userForAuthenticationDTO)
        {
            if (!await _serviceManager.AuthenticationService.ValidateUser(userForAuthenticationDTO))
                return Unauthorized();

            var tokenDTO = await _serviceManager.AuthenticationService.CreateToken(populateExp: true);


            return Ok(new { 
                AccessToken = tokenDTO.AccessToken, 
                RefreshToken = tokenDTO.RefreshToken 
            });
        }
        #endregion
    }
}
