using cron_que.Dtos;
using cron_que.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cron_que.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LikesController : ControllerBase
{
    private readonly LikesService likesService;

    public LikesController(LikesService likesService)
    {
        this.likesService = likesService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateLike(CreateLikeDto dto)
    {
        var like = await likesService.CreateLikeAsync(dto with { UserId = this.GetUserId() });
        return CreatedAtAction(nameof(GetAllLikes), new { }, like);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllLikes()
    {
        return Ok(await likesService.GetAllLikesAsync());
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLike(Guid id)
    {
        var like = await likesService.GetLikeByIdAsync(id);
        if (like is null)
        {
            return NotFound();
        }
        if (like.UserId != this.GetUserId())
        {
            return Forbid();
        }

        var deleted = await likesService.DeleteLikeAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
