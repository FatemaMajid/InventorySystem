using AutoMapper;
using FluentValidation;
using InventorySystem.API.Middleware;
using InventorySystem.Application.Common.Behaviors;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.Features.Branches.Commands.CreateBranch;
using InventorySystem.Application.Features.Branches.Queries.GetAllBranches;
using InventorySystem.Application.Features.InventorySessions.Import.Confirm;
using InventorySystem.Application.Features.InventorySessions.Import.Preview;
using InventorySystem.Infrastructure.Persistence.Contexts;
using InventorySystem.Infrastructure.Seed;
using MediatR;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

// Inventory Import
builder.Services.AddScoped<InventoryImportPreviewService>();
builder.Services.AddScoped<InventoryImportConfirmService>();

// Localization
builder.Services.AddLocalization();

// MediatR
builder.Services.AddMediatR(
    typeof(GetAllBranchesQuery).Assembly);

// FluentValidation
builder.Services.AddValidatorsFromAssembly(
    typeof(CreateBranchCommandValidator).Assembly);

builder.Services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(ValidationBehavior<,>));

// AutoMapper
builder.Services.AddAutoMapper(
    typeof(GetAllBranchesQuery).Assembly);

// Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Request Localization
var supportedCultures = new[]
{
    new CultureInfo("ar"),
    new CultureInfo("en")
};

var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("ar"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};

app.UseRequestLocalization(localizationOptions);

// Exception Handling
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Seed Master Data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    await BranchSeeder.SeedAsync(context);
    await StoreSeeder.SeedAsync(context);
    await UnitSeeder.SeedAsync(context);
}

app.Run();