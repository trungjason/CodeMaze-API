using AutoMapper;
using Contacts.Interfaces;
using Entities.ConfigurationModels;
using Entities.Exceptions.BadRequest;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Service.Contracts;
using Shared.DataTransferObjects;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Unicode;
using System.Xml.Linq;

namespace Service
{
    public class AuthenticationService : IAuthenticationService
    {
        #region Constructor
        private readonly UserManager<User> _userManager;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        private readonly IOptionsMonitor<JwtConfiguration> _configuration;
        private readonly JwtConfiguration _jwtConfiguration;

        private User? _user;

        public AuthenticationService(
            ILoggerManager logger, 
            IMapper mapper, 
            UserManager<User> userManager,
            IOptionsMonitor<JwtConfiguration> configuration)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;

            _jwtConfiguration = _configuration.Get(JwtConfiguration.Section);
        }
        #endregion

        public async Task<IdentityResult> RegisterUser(UserForRegistrationDTO userForRegistrationDTO)
        {
            var user = _mapper.Map<User>(userForRegistrationDTO);

            var result = await _userManager.CreateAsync(user, userForRegistrationDTO.Password);

            // TODO: Inject RoleManager then use RoleExistsAsync to check role before add to db
            if (result.Succeeded)
                await _userManager.AddToRolesAsync(user, userForRegistrationDTO.Roles);

            return result;
        }

        public async Task<bool> ValidateUser(UserForAuthenticationDTO userForAuth)
        {
            _user = await _userManager.FindByNameAsync(userForAuth.UserName);


            var result = (_user != null && await _userManager.CheckPasswordAsync(_user, userForAuth.Password));

            if (!result)
            {
                _logger.LogWarn($"{nameof(ValidateUser)}: Authentication failed. Wrong user name or password.");
            };

            return result;
        }

        #region Create Token
        public async Task<TokenDTO> CreateToken(bool populateExp)
        {
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims();

            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

            var refreshToken = GenerateRefreshToken();
            _user.RefreshToken = refreshToken;

            if (populateExp)
            {
                _user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            };

            await _userManager.UpdateAsync(_user);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return new TokenDTO(accessToken,refreshToken);
        }

        private SigningCredentials GetSigningCredentials()
        {
            var jwtSecretKey = _jwtConfiguration.SecretKey;
            //var jwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

            var key = Encoding.UTF8.GetBytes(jwtSecretKey);
            var secret = new SymmetricSecurityKey(key);

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, _user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(_user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            };

            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var tokenOptions = new JwtSecurityToken
            (
                issuer: _jwtConfiguration.ValidIssuer,
                audience: _jwtConfiguration.ValidAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtConfiguration.Expires)),
                signingCredentials: signingCredentials
            );

            return tokenOptions;
        }
        #endregion

        #region Refresh Token
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);

                return Convert.ToBase64String(randomNumber);
            }
        }

        private ClaimsPrincipal GetClaimsPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,

                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.SecretKey)),

                ValidateLifetime = true,
                ValidIssuer = _jwtConfiguration.ValidIssuer,
                ValidAudience = _jwtConfiguration.ValidAudience
            };

            var tokenHandlers = new JwtSecurityTokenHandler();

            SecurityToken securityToken;

            var principal = tokenHandlers.ValidateToken(token, tokenValidationParameters, out securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken
                .Header.Alg
                .Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        public async Task<TokenDTO> RefreshToken(TokenDTO tokenDTO)
        {
            var claimsPrincipal = GetClaimsPrincipalFromExpiredToken(tokenDTO.AccessToken);

            var user = await _userManager.FindByNameAsync(claimsPrincipal.Identity.Name);

            if (user == null || user.RefreshToken != tokenDTO.RefreshToken 
                || user.RefreshTokenExpiryTime <= DateTime.Now)
                throw new RefreshTokenBadRequestException();

            _user = user;

            // we don’t want to update the expiry time of the refresh token thus sending false as a parameter.
            return await CreateToken(populateExp: false);
        }
        #endregion
    }
}
