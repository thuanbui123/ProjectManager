using CORE.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace CORE.Exceptions;

public class HandleExceptionMiddleware
{
    private RequestDelegate _next;
    public HandleExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidateException ex)
        {
            var serviceResult = new ServiceResultModel();
            context.Response.StatusCode = 400;
            serviceResult.Errors.Add(ex.Message);
            var res = JsonConvert.SerializeObject(serviceResult);
            await context.Response.WriteAsync(res);
        }
        catch (Exception ex)
        {
            var serviceResult = new ServiceResultModel();
            context.Response.StatusCode = 500;
            serviceResult.Errors.Add(ex.Message);
            var res = JsonConvert.SerializeObject(serviceResult);
            await context.Response.WriteAsync(res);
        }
    }
}
