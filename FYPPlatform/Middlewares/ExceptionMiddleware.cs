using FYPPlatform.Web.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FYPPlatform.Web.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, IWebHostEnvironment hostingEnvironment, ILoggerFactory loggerFactory)
        {
            _next = next;
            _hostingEnvironment = hostingEnvironment;
            _logger = loggerFactory.CreateLogger<ExceptionMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                int statusCode;

                if (e is TimeoutException)
                {
                    statusCode = (int)HttpStatusCode.ServiceUnavailable;
                    _logger.LogError(e, e.Message);
                }
                else if (e is StorageErrorException)
                {
                    var storageErrorException = (StorageErrorException)e;
                    if (storageErrorException.StatusCode < 500)
                    {
                        _logger.LogWarning(e, e.Message);
                    }
                    else
                    {
                        _logger.LogError(e, e.Message);
                    }
                    statusCode = storageErrorException.StatusCode;
                }
                else if (e is BadRequestException)
                {
                    _logger.LogWarning(e, e.Message);
                    statusCode = 400;
                }
                else
                {
                    statusCode = 500;
                    _logger.LogError(e, e.Message);
                }

                context.Response.Clear();
                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";

                string body = e.Message;
                if (!_hostingEnvironment.IsProduction())
                {
                    var response = new
                    {
                        Exception = e.ToString()
                    };

                    body = JsonConvert.SerializeObject(response);
                }
                await context.Response.WriteAsync(body);
            }
        }
    }
}
