using System.Net;
using System.Text.Json;
using FluentValidation;
using SupportTickets.Api.Responses;
using SupportTickets.Domain.Exceptions;

namespace SupportTickets.Api.Middlewares;

public sealed class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Erro não tratado durante o processamento da requisição.");
            await HandleExceptionAsync(context, exception);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = exception switch
        {
            ValidationException validationException => (
                StatusCode: HttpStatusCode.BadRequest,
                Payload: ApiResponse<object>.Fail(
                    "Falha de validação.",
                    validationException.Errors.Select(x => x.ErrorMessage).Distinct().ToArray())),

            NotFoundException notFoundException => (
                StatusCode: HttpStatusCode.NotFound,
                Payload: ApiResponse<object>.Fail("Recurso não encontrado.", notFoundException.Message)),

            ForbiddenException forbiddenException => (
                StatusCode: HttpStatusCode.Forbidden,
                Payload: ApiResponse<object>.Fail("Acesso negado.", forbiddenException.Message)),

            UnauthorizedException unauthorizedException => (
                StatusCode: HttpStatusCode.Unauthorized,
                Payload: ApiResponse<object>.Fail("Não autorizado.", unauthorizedException.Message)),

            DomainException domainException => (
                StatusCode: HttpStatusCode.BadRequest,
                Payload: ApiResponse<object>.Fail("Falha de negócio.", domainException.Message)),

            _ => (
                StatusCode: HttpStatusCode.InternalServerError,
                Payload: ApiResponse<object>.Fail("Erro interno no servidor.", "Ocorreu um erro inesperado ao processar a requisição."))
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)response.StatusCode;

        var json = JsonSerializer.Serialize(response.Payload);
        await context.Response.WriteAsync(json);
    }
}
