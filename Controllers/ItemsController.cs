using cron_que.Dtos;
using cron_que.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cron_que.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ItemsController : ControllerBase
{
    private readonly ItemsService itemsService;

    public ItemsController(ItemsService itemsService)
    {
        this.itemsService = itemsService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateItem(CreateItemDto dto)
    {
        try
        {
            var item = await itemsService.CreateItemAsync(dto with { UserId = this.GetUserId() });
            return CreatedAtAction(nameof(GetItemById), new { id = item.Id }, item);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllItems()
    {
        return Ok(await itemsService.GetAllItemsAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetItemById(Guid id)
    {
        var item = await itemsService.GetItemByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateItemStatus(Guid id, UpdateItemStatusDto dto)
    {
        var existing = await itemsService.GetItemByIdAsync(id);
        if (existing is null)
        {
            return NotFound();
        }
        if (existing.UserId != this.GetUserId())
        {
            return Forbid();
        }

        var item = await itemsService.UpdateItemStatusAsync(id, dto);
        return item is null ? NotFound() : Ok(item);
    }
}
