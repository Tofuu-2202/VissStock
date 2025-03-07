using VisssStock.Application.Services.AuthService;
using VisssStock.Application.Services.UserServices;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using System.IO.Compression;
using VisssStock.Infrastructure.HTTP.SSO.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;
using VisssStock.Application.Services.RoleServices;
using VisssStock.Application.Services.MenuServices;
using VisssStock.Application.Services.PermissionServices;
using MongoDB.Driver;
using VisssStock.Infrastructure.Data;
using VisssStock.Application.Services.ExchangeServices;
using VisssStock.Application.Services.TypeServices;
using VisssStock.Application.Services.StockServices;
using VisssStock.Application.Services.StockGroupServices;
using VisssStock.Application.Services.ScreenerServices;
using VisssStock.Application.Services.TradingViewAPI;
using VisssStock.Application.Services.StockGroupStockServices;
using VisssStock.Application.Services.IndicatorServices;
using VisssStock.Application.Services.IntervalServices;
using VisssStock.Application.Services.StockGroupIndicatorServices;
using VisssStock.Application.Services.IndicatorDraft;
using VisssStock.Application.Services.ConditonGroupService;
using VisssStock.Application.Services.AlertLogService;
using VisssStock.Application.Services.TelegramBotService;
using VisssStock.Infrastructure.Repository;
using VisssStock.Infrastructure.Repository.TransactionRepository;
using VisssStock.Application.Services.TransactionServices;
using VisssStock.Application.Utility;
using Pomelo.EntityFrameworkCore.MySql;
using AutoMapper;
using VisssStock.Application.Interfaces;

namespace VisssStock.WebAPI
{
    public class Startup
    {
        public IConfiguration configRoot { get; }

        public Startup(IConfiguration configuration)
        {
            configRoot = configuration;
        }

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();
            configRoot = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // services.AddScoped<IAuthService, AuthServiceImpl>();
            // services.AddScoped<IUserService, UserServiceImpl>();
            // services.AddScoped<IRoleService, RoleServiceImpl>();
            // services.AddScoped<IPermissionService, PermissionServiceImpl>();
            // services.AddScoped<IMenuService, MenuServiceImpl>();
            // // Add service
            // services.AddScoped<IExchangeService, ExchangeServiceImpl>();
            // services.AddScoped<ITypeService, TypeServiceImpl>();
            // services.AddScoped<IStockService, StockService>();
            // services.AddScoped<IStockGroupService, StockGroupService>();
            // services.AddScoped<IScreenerService, ScreenerService>();
            // services.AddScoped<IStockGroupStockService, StockGroupStockService>();
            // services.AddScoped<IIndicatorService, IndicatorServiceImpl>();
            // services.AddScoped<IIntervalService, IntervalServiceImpl>();
            // services.AddScoped<IStockGroupIndicatorService, StockGroupIndicatorServiceImpl>();
            // services.AddScoped<IIndicatorDraftService, IndicatorDraftService>();
            // services.AddScoped<IConditionGroupService, ConditionGroupService>();
            // services.AddScoped<IAlertLogService, AlertLogService>();
            // services.AddScoped<ITelegramBotService, TelegramBotService>();

            // //transaction
            // services.AddScoped<ITransactionServices, TransactionServices>();
            services.AddScoped<TransactionRepository>();

            // Add repository
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // End add service
            services.AddAutoMapper(typeof(Program).Assembly);
            services.AddScoped<IJwtTokenProvider, JwtTokenProvider>();
            services.AddScoped<Helper>();

            services.AddHttpClient<ITradingViewService, TradingViewService>();

            services.AddOptions();
            services.AddHttpContextAccessor();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                    System.Text.Encoding.UTF8.GetBytes(configRoot.GetSection("AppSettings:TokenKeySecret").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddDistributedMemoryCache();
            services.AddSession();
            services.AddDbContext<DataContext>(options =>
            {
                options.UseMySql(configRoot["ConnectionStrings:DefaultConnection"], new MySqlServerVersion(new Version(8, 0, 21)));
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                builder =>
                {
                    builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("x-pagination");
                });
            });

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CMS API", Version = "v1" });
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "Standard Authorization header using the Bearer scheme, e.g. \"Bearer {token} \"",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.OperationFilter<SecurityRequirementsOperationFilter>();
            });

            services.AddSingleton<IMongoClient, MongoClient>(sp => new MongoClient(configRoot.GetConnectionString("MongoDB")));
            //services.AddSingleton<WebSocketManager.WebSocketHandler>();

            // Add gzip compression
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                //options.EnableForHttps = true;
                options.MimeTypes = new[]
                {
                    // Default
                    "text/plain",
                    "text/css",
                    "application/javascript",
                    "text/html",
                    "application/xml",
                    "text/xml",
                    "application/json",
                    "text/json",

                    // Custom
                    "image/svg+xml",
                    "application/font-woff2"
                };
            });

            services.Configure<HstsOptions>(options =>
            {
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });
        }

        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            app.UseSession();
            app.UseCors("AllowAll");
            app.UseCors();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
                // specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    // Expand details shown from default setting
                    c.DefaultModelExpandDepth(2);

                    // Smaller display onscreen
                    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);

                    // Display the example JSON by default
                    c.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Example);

                    // Make it so you don't have to keep clicking "Try It Now" to use a WebApi method.
                    c.EnableTryItOutByDefault();

                    // Disable highlighting as it really bogs down with large JSON loads.
                    c.ConfigObject.AdditionalItems.Add("syntaxHighlight", false);
                });
                //app.UseSwaggerUI(c =>
                //{
                //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CMS API V1");
                //    c.RoutePrefix = string.Empty;
                //});
            }

            app.UseHttpsRedirection();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseResponseCompression();

            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    const int cacheExpirationInSeconds = 60 * 60 * 24 * 30; //one month
                    ctx.Context.Response.Headers[HeaderNames.CacheControl] =
                    "public,max-age=" + cacheExpirationInSeconds;
                }
            });
            app.UseAuthentication();
            app.UseAuthorization();
            // app.UseAuthorization(); 
            //app.UseWebSockets();
            //app.UseMiddleware<WebSocketManager.WebSocketManagerMiddleware>();
            app.MapControllers();
            app.Run();
        }
    }
}
