using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Demo.WebApi.Extensions.Setup
{
    /// <summary>
    /// JWT 认证注册
    /// </summary>
    public static class JwtSetup
    {
        public static IServiceCollection AddJwtSetup(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JwtSetting:ValidIssuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["JwtSetting:ValidAudience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["JwtSetting:IssuerSigningKey"])),
                    ValidateLifetime = true,
                    SaveSigninToken = true,
                    ClockSkew = TimeSpan.FromSeconds(1)
                };
            });

            return services;
        }
    }
}
