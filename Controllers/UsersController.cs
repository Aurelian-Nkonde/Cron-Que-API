using cron_que.Dtos;
using cron_que.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cron_que.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly UsersService usersService;

    public UsersController(UsersService usersService)
    {
        this.usersService = usersService;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreateUser(CreateUserDto dto)
    {
        var user = await usersService.CreateUserAsync(dto);
        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        return Ok(await usersService.GetAllUsersAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var user = await usersService.GetUserByIdAsync(id);
        return user is null ? NotFound() : Ok(user);
    }
}
