using AutoMapper;
using FluentValidation;
using InventorySystem.Application.Common.Behaviors;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.Features.Branches.Commands.CreateBranch;
using InventorySystem.Application.Features.Branches.Queries.GetAllBranches;
using InventorySystem.Infrastructure.Persistence.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using InventorySystem.API.Middleware;

var builder = WebApplication.CreateBuilder(args);


// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IApplicationDbContext, ApplicationDbContext>();


// MediatR
builder.Services.AddMediatR(typeof(GetAllBranchesQuery).Assembly);


// FluentValidation
builder.Services.AddValidatorsFromAssembly(
    typeof(CreateBranchCommandValidator).Assembly);


// MediatR Validation Pipeline
builder.Services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(ValidationBehavior<,>));


// AutoMapper
builder.Services.AddAutoMapper(
    typeof(GetAllBranchesQuery).Assembly);


// Controllers
builder.Services.AddControllers();


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


// Exception Handling
app.UseMiddleware<ExceptionHandlingMiddleware>();


// Configure HTTP pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();