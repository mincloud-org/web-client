using System.Net;
using Web.Server.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json.Nodes;
using Web.Server.ActionResults;

namespace Web.Server.Filters;


public class HttpGlobalExceptionFilter(ILogger<HttpGlobalExceptionFilter> logger, IWebHostEnvironment env) : IExceptionFilter
{
    private readonly ILogger<HttpGlobalExceptionFilter> _logger = logger;
    public void OnException(ExceptionContext context)
    {
        logger.LogError(new EventId(context.Exception.HResult), context.Exception, context.Exception.Message);
        if (context.Exception.GetType() == typeof(InvalidInputException))
        {
            var problemDetails = new HttpValidationProblemDetails()
            {
                Instance = context.HttpContext.Request.Path,
                Status = StatusCodes.Status400BadRequest,
                Detail = "Please refer to the errors property for additional details."
            };

            switch (context.Exception.InnerException)
            {
                case null:
                    problemDetails.Errors.Add("Message", [context.Exception.Message]);
                    break;
            }
            context.Result = new BadRequestObjectResult(problemDetails);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
        else
        {
            var json = new JsonObject
            {
                ["Message"] = "An unexpected error occurred. Please try again later."
            };
            if (env.IsDevelopment())
            {
                json.Add("Exception", JsonNode.Parse(System.Text.Json.JsonSerializer.Serialize(context.Exception)));
            }
            context.Result = new InternalServerErrorObjectResult(json);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }
        context.ExceptionHandled = true;
    }
}
