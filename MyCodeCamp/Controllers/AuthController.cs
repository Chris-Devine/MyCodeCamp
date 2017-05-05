using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MyCodeCamp.Data;
using MyCodeCamp.Data.Entities;
using MyCodeCamp.Filters;
using MyCodeCamp.Models;

namespace MyCodeCamp.Controllers
{
    public class AuthController : Controller
    {
        private ILogger<AuthController> _logger;
        private CampContext _context;
        private SignInManager<CampUser> _signInManager;
        private UserManager<CampUser> _userMgr;
        private IPasswordHasher<CampUser> _hasher;
        private IConfigurationRoot _config;

        public AuthController(ILogger<AuthController> logger, 
            CampContext context, 
            SignInManager<CampUser> signInManager, 
            IPasswordHasher<CampUser> hasher, 
            UserManager<CampUser> userMgr,
            IConfigurationRoot config)
        {
            _logger = logger;
            _context = context;
            _signInManager = signInManager;
            _hasher = hasher;
            _userMgr = userMgr;
            _config = config;
        }

        [HttpPost("api/auth/login")]
        [ValidateModel]
        public async Task<IActionResult> Login([FromBody] CredentialModel model)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
                if (result.Succeeded)
                {
                    return Ok();
                }
            }
            catch (Exception ex)
            {
               _logger.LogError($"Exception thrown while logging in: {ex}");
            }
            return BadRequest("Failed to login");
        }

        [HttpPost("api/auth/token")]
        [ValidateModel]
        public async Task<IActionResult> createToken([FromBody] CredentialModel model)
        {
            try
            {
                var user = await _userMgr.FindByNameAsync(model.UserName);
                if (user != null)
                {

                    var userClaims = await _userMgr.GetClaimsAsync(user);

                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                        new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                        new Claim(JwtRegisteredClaimNames.Email, user.Email)
                    }.Union(userClaims);

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    if (_hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) ==
                        PasswordVerificationResult.Success)
                    {
                        var token = new JwtSecurityToken(
                            issuer: _config["Tokens:Issuer"],
                            audience: _config["Tokens:Audience"],
                            claims: claims,
                            expires: DateTime.UtcNow.AddMinutes(15),
                            signingCredentials: creds
                        );

                        return Ok(new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        });
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown creating a JWT: {ex}");
            }
            return BadRequest("Failed to generating a token");
        }
    }
}
