using AuthApi.DTOs;
using AuthApi.Exceptions.ExceptionsTypes;
using Microsoft.AspNetCore.Diagnostics;

namespace AuthApi.Exceptions
{
    public class HandleAllExceptions : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {

            int StatusCode;
            
            switch (exception)
            {
                case UserAlreadyExistException:  
                    httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                    StatusCode = StatusCodes.Status400BadRequest;
                    break;

                case UserNotFoundException: 
                    httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                    StatusCode = StatusCodes.Status400BadRequest;
                    break;

                case DailyAttemptsReachedException:
                    httpContext.Response.StatusCode= StatusCodes.Status400BadRequest;
                    StatusCode = StatusCodes.Status400BadRequest;
                    break;



                default:
                    httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    StatusCode = StatusCodes.Status500InternalServerError;

                    break;
            }

            if (StatusCode == StatusCodes.Status500InternalServerError)
            {
                await httpContext.Response.WriteAsJsonAsync(new
                {
                    error = "An unexpected error occurred",
                    statusCode = StatusCode
                }, cancellationToken);
            }
            else
            {
                await httpContext.Response.WriteAsJsonAsync(new
                {
                    error = exception.Message,
                    statusCode = StatusCode
                }, cancellationToken);
            }
                

            return true;
        }
    }
}
