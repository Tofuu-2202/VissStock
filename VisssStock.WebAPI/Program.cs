// using Hellang.Middleware.ProblemDetails;
// using VisssStock.Infrastructure;
// using VisssStock.Domain.DataObjects;
// using Microsoft.EntityFrameworkCore;
// using System.Configuration;

// var builder = WebApplication.CreateBuilder(args);
// var startup = new VisssStock.WebAPI.Startup(builder.Environment);
// startup.ConfigureServices(builder.Services); // calling ConfigureServices method
// var app = builder.Build();
// app.UseSwagger();
// app.UseSwaggerUI();
// startup.Configure(app, builder.Environment); // calling Configure method


using Hellang.Middleware.ProblemDetails;
using VisssStock.Infrastructure;
using VisssStock.Domain.DataObjects;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace VisssStock.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var startup = new Startup(builder.Environment);
            startup.ConfigureServices(builder.Services);
            var app = builder.Build();
            app.UseSwagger();
            app.UseSwaggerUI();
            startup.Configure(app, builder.Environment);
            app.Run();
        }
    }
}