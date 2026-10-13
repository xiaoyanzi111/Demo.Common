using System;
using System.IO;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Demo.Common;
using Demo.Extensions;
using Demo.TaskServer.HostedService;
using Demo.WebApi.Extensions.Middleware;
using Demo.WebApi.Extensions.Setup;
using Demo.WebApi.Filters;
using Demo.WebApi.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

// ============================================================
//  Demo.WebApi —— 应用宿主入口
//  技术栈：.NET 8 + Autofac + SqlSugar(SQL Server) + JWT + Swagger + NLog
// ============================================================

var logger = NLogBuilder.ConfigureNLog("Configs/nlog.config").GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    #region 配置加载（appsettings.json / Configs/*.json / 环境配置）
    builder.Configuration
        .AddJsonFile("Configs/redis.json", optional: true, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

    // 同步到全局静态配置入口，方便在任意层读取
    AppConfig.Configuration = builder.Configuration;
    AppSettings.Configuration = builder.Configuration;
    #endregion

    #region 日志（NLog）
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(LogLevel.Information);
    builder.Logging.AddConsole();
    #endregion

    #region 宿主（Autofac / NLog / Windows 服务）
    builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
    builder.Host.UseNLog();
    builder.Host.UseWindowsService();
    builder.Host.ConfigureContainer<ContainerBuilder>(container => container.RegisterDemoServices());
    #endregion

    #region 服务注册
    builder.Services.AddSqlSugarSetup(builder.Configuration);
    builder.Services.AddJwtSetup(builder.Configuration);
    builder.Services.AddAutoMapperSetup();
    builder.Services.AddSwaggerSetup();
    builder.Services.AddCorsSetup(builder.Configuration);

    builder.Services.AddControllers(options =>
    {
        // Swagger 分组约定
        options.Conventions.Add(new ApiGroupConvention());
        // 全局异常 + 全局模型验证
        options.Filters.Add<GlobalExceptionsFilter>();
        options.Filters.Add<GlobalActionMonitorFilter>();
    })
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

    // 常驻后台周期任务（示例，如不需要可注释）
    builder.Services.AddHostedService<SampleTimedService>();
    #endregion

    var app = builder.Build();

    #region 请求管道
    if (app.Environment.IsDevelopment())
    {
        app.UseSwaggerSetup(app.Environment);
    }

    app.UseCors(CorsSetup.PolicyName);
    app.UseMiddleware<RequestOptionMiddleware>();

    // 静态文件
    app.UseStaticFiles();

    // 上传目录（自动创建并映射到 /UpLoad）
    string uploadPath = Path.Combine(AppContext.BaseDirectory, "UpLoad");
    if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(uploadPath),
        RequestPath = "/UpLoad"
    });

    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    #endregion

    Log.Info("服务启动成功");
    app.Run();
}
catch (Exception ex)
{
    Log.Error(ex, "服务启动异常");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}
