using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.Api.CORS;

public static class CorsExtensions
{
    private const string allowedOriginSetting = "AllowedOrigin";
    public static IServiceCollection AddGameStoreCors(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddCors(options =>
        {
            options.AddDefaultPolicy(corsBuilder =>
            {
                var allowedOrigin = configuration[allowedOriginSetting] ??
                throw new InvalidOperationException("AllowedOrigin is not set");
                corsBuilder.WithOrigins(allowedOrigin).AllowAnyHeader().AllowAnyMethod();
            });
        });
    }


}