using AspNetCoreRateLimit;
using Fricks.Middlewares;
using Fricks.Repository;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Fricks
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWebAPIService(this IServiceCollection services)
        {
            //Add Service

            //Add Middleware
            services.AddExceptionHandler<ExceptionHandlerMiddleware>();
            services.AddSingleton<PerformanceMiddleware>();
            services.AddSingleton<Stopwatch>();
            services.AddProblemDetails();

            //Add DbContext
            services.AddDbContext<FricksDbContext>(options =>
            {
                options.UseSqlServer(Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTION_STRING"));
            });

            //Add Brute Force setting
            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            return services;
        }
    }
}
