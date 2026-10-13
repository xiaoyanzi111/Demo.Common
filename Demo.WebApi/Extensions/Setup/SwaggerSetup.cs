using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Demo.WebApi.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Demo.WebApi.Extensions.Setup
{
    /// <summary>
    /// Swagger 注册（按 ApiGroupNames 自动分组 + JWT 鉴权支持）
    /// </summary>
    public static class SwaggerSetup
    {
        public static IServiceCollection AddSwaggerSetup(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                // XML 注释
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                    options.IncludeXmlComments(xmlPath, true);

                // 按分组枚举生成文档
                typeof(ApiGroupNames).GetFields().Skip(1).ToList().ForEach(f =>
                {
                    var info = f.GetCustomAttributes(typeof(GroupInfoAttribute), false).OfType<GroupInfoAttribute>().FirstOrDefault();
                    options.SwaggerDoc(f.Name, new OpenApiInfo
                    {
                        Title = info?.Title ?? f.Name,
                        Version = info?.Version ?? "v1",
                        Description = info?.Description
                    });
                });

                // 未标注分组的接口
                options.SwaggerDoc("NoGroup", new OpenApiInfo { Title = "未分组接口" });

                // JWT 输入框
                options.OperationFilter<SecurityRequirementsOperationFilter>();
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Description = "请输入带有 Bearer 前缀的 JWT Token",
                    BearerFormat = "JWT",
                    Scheme = "Bearer",
                    Name = "Authorization"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        Array.Empty<string>()
                    }
                });

                options.DocInclusionPredicate((docName, apiDescription) =>
                {
                    if (docName == "NoGroup")
                        return string.IsNullOrEmpty(apiDescription.GroupName);

                    return apiDescription.GroupName == docName;
                });
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerSetup(this IApplicationBuilder app, IHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                typeof(ApiGroupNames).GetFields().Skip(1).ToList().ForEach(f =>
                {
                    var info = f.GetCustomAttributes(typeof(GroupInfoAttribute), false).OfType<GroupInfoAttribute>().FirstOrDefault();
                    options.SwaggerEndpoint($"./{f.Name}/swagger.json", info != null ? info.Title : f.Name);
                });
                options.SwaggerEndpoint("/swagger/NoGroup/swagger.json", "未分组接口");
            });

            return app;
        }
    }
}
