using AutoMapper;
using FluentValidation;
using InventorySystem.Application.Common.Behaviors;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.Features.Branches.Commands.CreateBranch;
using InventorySystem.Application.Features.Branches.Queries.GetAllBranches;
using InventorySystem.Application.Features.InventorySessions.Import.Preview;
using InventorySystem.Infrastructure.Persistence.Contexts;
using InventorySystem.Infrastructure.Seed;
using MediatR;
using Microsoft.EntityFrameworkCore;
using InventorySystem.API.Middleware;

var builder = WebApplication.CreateBuilder(args);


// =====================================================
// Database
// =====================================================

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IApplicationDbContext, ApplicationDbContext>();


// =====================================================
// Inventory Import Preview
// =====================================================

builder.Services.AddScoped<InventoryImportPreviewService>();


// =====================================================
// MediatR
// =====================================================

builder.Services.AddMediatR(
    typeof(GetAllBranchesQuery).Assembly);


// =====================================================
// FluentValidation
// =====================================================

builder.Services.AddValidatorsFromAssembly(
    typeof(CreateBranchCommandValidator).Assembly);


// =====================================================
// MediatR Validation Pipeline
// =====================================================

builder.Services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(ValidationBehavior<,>));


// =====================================================
// AutoMapper
// =====================================================

builder.Services.AddAutoMapper(
    typeof(GetAllBranchesQuery).Assembly);


// =====================================================
// Controllers
// =====================================================

builder.Services.AddControllers();


// =====================================================
// Swagger
// =====================================================

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


// =====================================================
// Exception Handling
// =====================================================

app.UseMiddleware<ExceptionHandlingMiddleware>();


// =====================================================
// Configure HTTP pipeline
// =====================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


// =====================================================
// Seed Master Data
// =====================================================

using (var scope = app.Services.CreateScope())
{
    var context =
        scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

    // Branches must be seeded first
    await BranchSeeder.SeedAsync(context);

    // Stores depend on BranchCode
    await StoreSeeder.SeedAsync(context);

    // Units must be seeded before Products
    await UnitSeeder.SeedAsync(context);
}


app.Run();