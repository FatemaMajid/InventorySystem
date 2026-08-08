using InventorySystem.Application.DTOs.Branch;
using MediatR;

namespace InventorySystem.Application.Features.Branches.Queries.GetAllBranches;

public record GetAllBranchesQuery : IRequest<List<BranchDto>>;