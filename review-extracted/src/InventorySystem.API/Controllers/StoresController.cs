using InventorySystem.Application.DTOs.Store;
using InventorySystem.Application.Features.Stores.Commands.CreateStore;
using InventorySystem.Application.Features.Stores.Commands.DeleteStore;
using InventorySystem.Application.Features.Stores.Commands.UpdateStore;
using InventorySystem.Application.Features.Stores.Queries.GetAllStores;
using InventorySystem.Application.Features.Stores.Queries.GetStoreById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StoresController : ControllerBase
{
    private readonly IMediator _mediator;

    public StoresController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = "Permission:Store.Create")]
    public async Task<IActionResult> Create([FromBody] CreateStoreDto dto)
    {
        var command = new CreateStoreCommand
        {
            Store = dto
        };

        var id = await _mediator.Send(command);

        return Created($"api/Stores/{id}", id);
    }

    [HttpGet]
    [Authorize(Policy = "Permission:Store.View")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllStoresQuery());
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = "Permission:Store.View")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(
            new GetStoreByIdQuery { Id = id });

        if (result == null)
            return NotFound(new { message = "Store not found." });

        return Ok(result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "Permission:Store.Edit")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateStoreDto dto)
    {
        var command = new UpdateStoreCommand
        {
            Id = id,
            Store = dto
        };

        var result = await _mediator.Send(command);

        if (!result)
            return NotFound(new { message = "Store not found." });

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "Permission:Store.Edit")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _mediator.Send(
            new DeleteStoreCommand { Id = id });

        if (!result)
            return NotFound(new { message = "Store not found." });

        return NoContent();
    }
}