using Flushed_API.Dtos;
using Flushed_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using System.Security.Claims;
using System;
using System.Text;


namespace Flushed_API.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly JwtSettings _jwtSettings;

        public AccountController(UserManager<User> userManager, JwtSettings jwtSettings)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
        }

        [HttpPost]
        [Route("api/register")]
        public async Task<IActionResult> Register(UserRegistrationDto registration)
        {
            var user = new User
            {
                UserName = registration.Email,
                Email = registration.Email,
            };

            var result = await _userManager.CreateAsync(user, registration.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors });
            }

            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);
            var tokenHandler = new JwtSecurityTokenHandler();
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim("id", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                // ... other claims ...
            }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(descriptor);
            return Ok(new { token = tokenHandler.WriteToken(token) });
        }
    }
}
