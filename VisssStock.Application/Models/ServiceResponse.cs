using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System.Net;

namespace VisssStock.Application.Models
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected
            string message = exception.Message;
            string detailedMessage = exception.ToString(); // Include detailed error information

            // Check and categorize exceptions
            Exception baseException = exception.GetBaseException();
            if (baseException is MySqlException mysqlException)
            {
                code = HttpStatusCode.BadRequest; // or another status code as needed
                message = $"MySQL error: {mysqlException.Message}";
                detailedMessage = mysqlException.ToString();
            }
            else if (baseException is NotFoundException)
            {
                code = HttpStatusCode.NotFound;
                message = baseException.Message;
            }
            else if (baseException is UnauthorizedAccessException)
            {
                code = HttpStatusCode.Unauthorized;
                message = baseException.Message;
            }
            else if (baseException is DbUpdateException dbUpdateException && dbUpdateException.InnerException is MySqlException innerMysqlException)
            {
                code = HttpStatusCode.BadRequest;
                message = $"MySQL error: {innerMysqlException.Message}";
                detailedMessage = innerMysqlException.ToString();
            }

            var response = new ServiceResponse<string>
            {
                ErrorCode = (int)code,
                Status = false,
                Message = message,
                Data = detailedMessage // Add detailed error information
            };

            var result = JsonConvert.SerializeObject(response);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            await context.Response.WriteAsync(result);
        }
    }

    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }

    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    public class ServiceResponse<T>
    {
        public T? Data { get; set; } 
        public int ErrorCode { get; set; } = 200;
        public bool Status { get; set; } = true;
        public string Message { get; set; } = "OK";

        public ServiceResponse(Boolean status)
        {
            if (!status)
            {
                Status = false;
                ErrorCode = 400;
            }
        }
        public ServiceResponse()
        {
        }

        public static ServiceResponse<T> Failure(int errorCode, string message)
        {
            return new ServiceResponse<T>
            {
                ErrorCode = errorCode,
                Status = false,
                Message = message
            };
        }

        public static ServiceResponse<T> Success(T data)
        {
            return new ServiceResponse<T>
            {
                ErrorCode = 200,
                Status = true,
                Message = "Success",
                Data = data
            };
        }
    }
}