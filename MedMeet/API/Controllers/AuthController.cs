﻿using Business_logic.Auth;
using Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/authorization")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private UserManager<User> userManager;
        private RoleManager<IdentityRole<int>> roleManage;
        private ITokenService token;

        public AuthController(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager, ITokenService tokenService)
        {
            this.userManager = userManager;
            roleManage = roleManager;
            token = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto model)
        {
            var existingUser = await userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return BadRequest("Користувач з такою електронною поштою вже існує.");
            }

            User user = new User { UserName = model.UserName, Email = model.Email, FullName = model.UserName, SpecialtyId = null, CabinetId = null};

            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            if (!await roleManage.RoleExistsAsync("Patient"))
            {
                await roleManage.CreateAsync(new IdentityRole<int>("Patient"));
            }

            await userManager.AddToRoleAsync(user, "Patient");

            return Ok("Реєстрація успішна.");
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            User user = await userManager.FindByEmailAsync(model.Email);
            if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
            {
                return Unauthorized("Невірна електронна пошта або пароль.");
            }

            string accessToken = await token.GenerateAccessTokenAsync(user);
            string refreshToken = token.GenerateRefreshToken();

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            });


            user.RefreshTokens.Add(new RefreshToken { Token = refreshToken, Expires = DateTime.UtcNow.AddDays(7), Created = DateTime.UtcNow, CreatedByIp = ipAddress});

            await userManager.UpdateAsync(user);

            return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken});
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            string refreshToken = request.RefreshToken;

            var user = await userManager.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken && rt.Expires > DateTime.UtcNow));

            if (user == null)
            {
                return Unauthorized("Недійсний або протермінований refresh токен.");
            }

            string newAccessToken = await token.GenerateAccessTokenAsync(user);
            string newRefreshToken = token.GenerateRefreshToken();

            var oldToken = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken && rt.Expires > DateTime.UtcNow);
            if (oldToken == null)
            {
                return Unauthorized("Недійсний або протермінований refresh токен.");
            }

            user.RefreshTokens.Remove(oldToken);

            string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken,
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress 
            });

            await userManager.UpdateAsync(user);

            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

    }
}
