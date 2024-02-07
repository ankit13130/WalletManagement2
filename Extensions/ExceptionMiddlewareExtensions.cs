using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using WalletManagement2.CustomException;
using WalletManagement2.CustomExceptions;
using WalletManagement2.Models;

namespace WalletManagement2.Extensions;

public static class ExceptionMiddlewareExtensions
{
    public static void ConfigureExceptionHandler(this IApplicationBuilder app/*, ILoggerManager logger*/)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    context.Response.StatusCode = contextFeature.Error switch
                    {
                        UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                        NotFoundException => StatusCodes.Status404NotFound,
                        ScopeNotFoundException => StatusCodes.Status403Forbidden,
                        //BadRequestException => StatusCodes.Status400BadRequest,
                        //RequestFailedException => contextFeature.Error.Message.Contains("409") ? StatusCodes.Status409Conflict : StatusCodes.Status500InternalServerError,
                        _ => StatusCodes.Status500InternalServerError
                    };
                    //logger.LogError($"Something went wrong: {contextFeature.Error}");
                    await context.Response.WriteAsync(new ErrorDetails()
                    {
                        StatusCode = context.Response.StatusCode,
                        //Message = contextFeature.Path,
                        Message = contextFeature.Error.Message,
                    }.ToString());
                }
            });
        });
    }
}
