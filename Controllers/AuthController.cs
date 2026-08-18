using cron_que.Dtos;
using cron_que.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cron_que.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly UsersService _usersService;
    private readonly TokenService _tokenService;

    public AuthController(UsersService usersService, TokenService tokenService)
    {
        _usersService = usersService;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _usersService.ValidateCredentialsAsync(dto.Email, dto.Password);
        if (user is null)
        {
            return Unauthorized("Invalid email or password.");
        }

        var (token, expiresAt) = _tokenService.CreateToken(user);
        return Ok(new AuthResponseDto(token, expiresAt));
    }
}
