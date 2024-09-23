using AspNetCoreRateLimit;
using Fricks.Middlewares;
using Fricks.Repository;
using Fricks.Repository.UnitOfWork;
using Fricks.Service.Services;
using Fricks.Service.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Fricks
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWebAPIService(this IServiceCollection services)
        {
            // add Service

            // add DI
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserService, UserService>();


            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IClaimsService, ClaimsService>();

            // add Middleware
            services.AddExceptionHandler<ExceptionHandlerMiddleware>();
            services.AddSingleton<PerformanceMiddleware>();
            services.AddSingleton<Stopwatch>();
            services.AddProblemDetails();

            // add Brute Force setting
            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            return services;
        }
    }
}
