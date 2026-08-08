using InventorySystem.Application.Features.Branches.Queries.GetAllBranches;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using InventorySystem.Application.DTOs.Branch;
using InventorySystem.Application.Features.Branches.Commands.CreateBranch;
using InventorySystem.Application.Features.Branches.Queries.GetBranchById;
using InventorySystem.Application.Features.Branches.Commands.UpdateBranch;
using InventorySystem.Application.Features.Branches.Commands.DeleteBranch;

namespace InventorySystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BranchesController : ControllerBase
{
    private readonly IMediator _mediator;

    public BranchesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllBranchesQuery());

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBranchDto dto)
    {
        var command = new CreateBranchCommand { Branch = dto };
        var id = await _mediator.Send(command);

        return Created(
                $"api/Branches/{id}",
                id);
    }

    [HttpGet("{id:int}")]
public async Task<IActionResult> GetById(int id)
{
    var result = await _mediator.Send(
        new GetBranchByIdQuery { Id = id });

    if (result == null)
        return NotFound();

    return Ok(result);
}
[HttpPut("{id:int}")]
public async Task<IActionResult> Update(
    int id,
    [FromBody] UpdateBranchDto dto)
{
    var command = new UpdateBranchCommand
    {
        Id = id,
        Branch = dto
    };

    var result = await _mediator.Send(command);

    if (!result)
        return NotFound();

    return NoContent();
}
[HttpDelete("{id:int}")]
public async Task<IActionResult> Delete(int id)
{
    var result = await _mediator.Send(
        new DeleteBranchCommand { Id = id });

    if (!result)
        return NotFound();

    return NoContent();
}
}