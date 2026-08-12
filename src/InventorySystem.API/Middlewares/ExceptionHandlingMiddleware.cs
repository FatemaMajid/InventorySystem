using FluentValidation;
using InventorySystem.Application.Common.Localization;
using Microsoft.AspNetCore.Localization;
using System.Net;
using System.Text.Json;

namespace InventorySystem.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await HandleValidationException(context, ex);
        }
        catch (Exception ex)
        {
            await HandleException(context, ex);
        }
    }

    private static async Task HandleValidationException(
        HttpContext context,
        ValidationException ex)
    {
        var language = GetLanguage(context);

        var errors = ex.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e =>
                    LocalizationService.Get(
                        e.ErrorMessage,
                        language))
                    .ToArray());

        await WriteResponse(
            context,
            HttpStatusCode.BadRequest,
            LocalizationService.Get(
                LocalizationKeys.Common.ValidationFailed,
                language),
            errors);
    }

    private static async Task HandleException(
        HttpContext context,
        Exception ex)
    {
        var language = GetLanguage(context);

        var message = ex is InvalidOperationException
            ? ex.Message
            : LocalizationService.Get(
                LocalizationKeys.Common.OperationFailed,
                language);

        await WriteResponse(
            context,
            HttpStatusCode.InternalServerError,
            message);
    }

    private static string GetLanguage(HttpContext context)
    {
        return context.Features
            .Get<IRequestCultureFeature>()?
            .RequestCulture
            .UICulture
            .TwoLetterISOLanguageName
            .ToLowerInvariant() ?? "ar";
    }

    private static async Task WriteResponse(
        HttpContext context,
        HttpStatusCode statusCode,
        string message,
        object? errors = null)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            statusCode = (int)statusCode,
            message,
            errors
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}