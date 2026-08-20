using System.Globalization;
using System.Text;
using Microsoft.OpenApi;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;

using InventorySystem.API.Middleware;
using InventorySystem.API.Authorization;

using InventorySystem.Infrastructure.Reports;

using InventorySystem.Application.Common.Behaviors;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.Features.Branches.Commands.CreateBranch;
using InventorySystem.Application.Features.Branches.Queries.GetAllBranches;
using InventorySystem.Application.Features.InventorySessions.Import.Confirm;
using InventorySystem.Application.Features.InventorySessions.Import.Preview;

using InventorySystem.Infrastructure.Authentication;
using InventorySystem.Infrastructure.Persistence.Contexts;
using InventorySystem.Infrastructure.Seed;

using MediatR;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;


// =====================================================
// Builder
// =====================================================

var builder = WebApplication.CreateBuilder(args);


// =====================================================
// Database
// =====================================================

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString(
            "DefaultConnection")));

builder.Services.AddScoped<
    IApplicationDbContext,
    ApplicationDbContext>();


// =====================================================
// Inventory Import
// =====================================================

builder.Services.AddScoped<
    InventoryImportPreviewService>();

builder.Services.AddScoped<
    InventoryImportConfirmService>();


// =====================================================
// Localization
// =====================================================

builder.Services.AddLocalization();


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

builder.Services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(ValidationBehavior<,>));


// =====================================================
// AutoMapper
// =====================================================

builder.Services.AddAutoMapper(
    typeof(GetAllBranchesQuery).Assembly);


// =====================================================
// Authentication Services
// =====================================================

builder.Services.AddScoped<
    IPasswordService,
    PasswordService>();

builder.Services.AddScoped<
    IJwtService,
    JwtService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<
    ICurrentUserService,
    CurrentUserService>();

builder.Services.AddSingleton<
    IAuthorizationPolicyProvider,
    PermissionPolicyProvider>();

builder.Services.AddScoped<
    IAuthorizationHandler,
    PermissionAuthorizationHandler>();

builder.Services.AddScoped<
    IDashboardExportService,
    DashboardExportService>();    

builder.Services.AddScoped<
    IComparisonExportService,
    ComparisonExportService>();


// =====================================================
// JWT Authentication
// =====================================================

var jwtSecret =
    builder.Configuration["Jwt:Secret"];

if (string.IsNullOrWhiteSpace(jwtSecret))
{
    throw new InvalidOperationException(
        "JWT Secret is not configured. " +
        "Please add Jwt:Secret to appsettings.json " +
        "or appsettings.Development.json.");
}

var jwtIssuer =
    builder.Configuration["Jwt:Issuer"]
    ?? "InventorySystem";

var jwtAudience =
    builder.Configuration["Jwt:Audience"]
    ?? "InventorySystem.Frontend";

builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,

                ValidateAudience = true,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtIssuer,

                ValidAudience = jwtAudience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            jwtSecret)),

                ClockSkew =
                    TimeSpan.FromMinutes(1)
            };
    });


// =====================================================
// Authorization
// =====================================================

builder.Services.AddAuthorization();


// =====================================================
// Controllers & Swagger
// =====================================================

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description =
                "Enter: Bearer {your JWT token}"
        });

    options.AddSecurityRequirement(
        document =>
            new OpenApiSecurityRequirement
            {
                [
                    new OpenApiSecuritySchemeReference(
                        "Bearer",
                        document)
                ] = []
            });
});


// =====================================================
// CORS
// =====================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


// =====================================================
// Build
// =====================================================

var app = builder.Build();


// =====================================================
// CORS
// =====================================================

app.UseCors("Frontend");


// =====================================================
// Request Localization
// =====================================================

var supportedCultures = new[]
{
    new CultureInfo("ar"),
    new CultureInfo("en")
};

var localizationOptions =
    new RequestLocalizationOptions
    {
        DefaultRequestCulture =
            new RequestCulture("ar"),

        SupportedCultures =
            supportedCultures,

        SupportedUICultures =
            supportedCultures
    };

app.UseRequestLocalization(
    localizationOptions);


// =====================================================
// Exception Handling
// =====================================================

app.UseMiddleware<
    ExceptionHandlingMiddleware>();


// =====================================================
// Swagger
// =====================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}


// =====================================================
// HTTPS
// =====================================================

app.UseHttpsRedirection();


// =====================================================
// Authentication
// IMPORTANT: Authentication must be before Authorization
// =====================================================

app.UseAuthentication();

app.UseAuthorization();


// =====================================================
// Controllers
// =====================================================

app.MapControllers();


// =====================================================
// Database Seed
// =====================================================

using (var scope = app.Services.CreateScope())
{
    var context =
        scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

    var configuration =
        scope.ServiceProvider
            .GetRequiredService<IConfiguration>();

    // Master Data
    await BranchSeeder.SeedAsync(
        context);

    await StoreSeeder.SeedAsync(
        context);

    await UnitSeeder.SeedAsync(
        context);

    // Roles & Permissions
    await AuthorizationSeeder.SeedAsync(
        context);

    // Initial Manager
    await UserSeeder.SeedAsync(
        context,
        configuration);
}


// =====================================================
// Run
// =====================================================

app.Run();