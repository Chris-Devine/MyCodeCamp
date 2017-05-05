using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

        public AuthController(ILogger<AuthController> logger, CampContext context, SignInManager<CampUser> signInManager)
        {
            _logger = logger;
            _context = context;
            _signInManager = signInManager;
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
    }
}
