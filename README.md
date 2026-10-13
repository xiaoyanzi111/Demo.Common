# Demo —— .NET 8 后端框架脚手架

基于原 `AnZhen.Common` 项目结构重构的 **.NET 8** 通用后端框架，**只保留架构、不含任何业务逻辑**，
数据库默认面向 **SQL Server**。你可以直接在此骨架上开发新项目。

---

## 一、技术栈

| 类别 | 选型 |
| --- | --- |
| 运行时 | .NET 8 |
| Web 框架 | ASP.NET Core Web API（最小宿主 + 扩展方法组织） |
| IoC / AOP | Autofac（构造函数注入 + 接口动态代理拦截器） |
| ORM | SqlSugar（`SqlSugarScope`，多库/多租户、二级缓存） |
| 数据库 | SQL Server（`DBType = 1`，可切换其他库） |
| 对象映射 | AutoMapper |
| 认证 | JWT（`Microsoft.AspNetCore.Authentication.JwtBearer`） |
| 接口文档 | Swagger（`Swashbuckle`，按枚举分组 + JWT 支持） |
| 日志 | NLog（文件 + 控制台） |
| 缓存 | Redis（CSRedisCore） |
| 定时任务 | Quartz（可动态增删改启停）+ `IHostedService` 常驻周期任务 |
| 表格 | NPOI |

---

## 二、解决方案结构

```
demo/
├── Demo.sln
├── Directory.Build.props            # 全局统一构建属性（TargetFramework=net8.0 等）
├── .editorconfig / .gitignore
│
├── Demo.Common/                     # ① 公共基础设施
│   ├── Caches/                      #   进程内缓存
│   ├── Constants/                   #   枚举、常量
│   ├── Helpers/                     #   AppSettings / HTTP / OAuth / GZip / 日志格式化
│   ├── JWT/                         #   JwtManager 令牌签发
│   ├── LogHelper/                   #   Log 静态日志门面（NLog）
│   ├── Security/                    #   MD5 / SHA / RSA / DES(CBC)
│   └── Utilities/                   #   密码工具
│
├── Demo.Core/                       # ② 核心层（与 Web 无关）
│   ├── DB/                          #   DBOptions / DBConfig（连接配置读取）
│   └── Cache/                       #   RedisServer / RedisCache(SqlSugar 二级缓存)
│
├── Demo.Extensions/                 # ③ 配置与对象扩展
│   ├── Config/AppConfig.cs          #   全局静态配置入口
│   ├── AppConfigurationExtensions.cs
│   └── Extension.cs                 #   ToJson / CountSize 等
│
├── Demo.Model/                      # ④ 数据模型
│   ├── Api/                         #   ApiResult / PageParm / PagedInfo
│   ├── Dto/                         #   入参出参 DTO
│   └── Entity/                      #   实体（BaseEntity / SampleEntity / ScheduledTask）
│
├── Demo.Utils/                      # ⑤ 通用工具
│   └── Excel/ExcelHelper.cs         #   NPOI 导入导出
│
├── Demo.Interfaces/                 # ⑥ 契约与实现层（重点）
│   ├── Repository/                  #   IBaseRepository<T> / BaseRepository<T>
│   ├── Service/                     #   IBaseService<T> / BaseService<T> / UnitOfWork
│   ├── IService/                    #   业务服务接口（示例 ISampleService）
│   ├── Service/                     #   业务服务实现（示例 SampleService）
│   └── Mapping/                     #   AutoMapper Profile
│
├── Demo.TaskServer/                 # ⑦ 定时任务 / 后台服务
│   ├── TaskScheduler/               #   JobBase / JobFactory
│   ├── Service/                     #   ITaskSchedulerServer / TaskSchedulerServer
│   ├── Jobs/                        #   具体 Job（示例 SampleJob）
│   └── HostedService/               #   常驻周期任务基类 + 示例
│
└── Demo.WebApi/                     # ⑧ 应用宿主（启动项）
    ├── Program.cs                   #   入口：配置 / 日志 / Autofac / 管道
    ├── appsettings.json             #   JWT / 数据库(SQL Server) / CORS 配置
    ├── Configs/                     #   nlog.config / redis.json
    ├── Controllers/                 #   ApiControllerBase / Health / Auth / Sample
    ├── Filters/                     #   全局异常 / 模型验证 / 授权
    ├── Extensions/Setup/            #   SqlSugar/JWT/Swagger/CORS/AutoMapper/Autofac 注册
    ├── Extensions/Middleware/       #   OPTIONS 预检中间件
    ├── Interceptors/                #   AOP 日志拦截器
    └── Swagger/                     #   分组枚举与约定
```

### 依赖关系（自上而下）

```
Demo.WebApi  ──► Demo.Interfaces ──► Demo.Core ──► Demo.Common ──► Demo.Model
      │                │                 │                             ▲
      │                └─────────────────┴──► Demo.Extensions ─────────┤
      └──► Demo.TaskServer ────────────────────────────────────────────┘
      └──► Demo.Utils
```

---

## 三、快速开始

### 1. 准备数据库（SQL Server）

修改 `Demo.WebApi/appsettings.json` 中的连接串：

```json
"DBS": {
  "MainDB": "MainDb",
  "MutiDBEnabled": false,
  "ConnectionEncrypt": false,
  "MutiDBConns": [
    {
      "ConnId": "MainDb",
      "DBType": 1,
      "Enabled": true,
      "Connection": "Server=127.0.0.1,1433;Database=DemoDb;User Id=sa;Password=xxx;TrustServerCertificate=True;Encrypt=False;"
    }
  ]
}
```

> - `DBType = 1` 即 **SQL Server**（0=MySql，1=SqlServer，2=Sqlite，3=Oracle，4=PostgreSQL）。
> - `ConnectionEncrypt = true` 时连接串会按 `DesCbcUtil` 解密（密文可用 `DesCbcUtil.Encode` 生成）。

示例表脚本（供 `SampleController` 演示使用，可自行删除）：

```sql
CREATE TABLE SAMPLE_TABLE (
    ID          BIGINT IDENTITY(1,1) PRIMARY KEY,
    NAME        NVARCHAR(100)  NULL,
    REMARK      NVARCHAR(500)  NULL,
    CREATE_TIME DATETIME       NULL,
    CREATE_BY   NVARCHAR(50)   NULL,
    UPDATE_TIME DATETIME       NULL,
    UPDATE_BY   NVARCHAR(50)   NULL,
    IS_DELETED  BIT            NOT NULL DEFAULT 0
);

CREATE TABLE SYS_TASKS_QZ (
    ID              NVARCHAR(50)  PRIMARY KEY,
    NAME            NVARCHAR(200) NULL,
    JOB_GROUP       NVARCHAR(100) NULL,
    ASSEMBLY_NAME   NVARCHAR(200) NULL,
    CLASS_NAME      NVARCHAR(200) NULL,
    CRON            NVARCHAR(100) NULL,
    TRIGGER_TYPE    INT           NULL,
    INTERVAL_SECOND INT           NULL,
    RUN_TIMES       INT           NULL,
    BEGIN_TIME      DATETIME      NULL,
    END_TIME        DATETIME      NULL,
    JOB_PARAMS      NVARCHAR(2000) NULL
);
```

### 2. Redis（可选）

`Demo.WebApi/Configs/redis.json`：

```json
"RedisConfig": {
  "Enabled": true,
  "RedisConnectionString": "127.0.0.1:6379,defaultDatabase=0,..."
}
```

> 没有 Redis 时把 `Enabled` 改为 `false` 即可（将不启用 SqlSugar 二级缓存）。

### 3. 运行

```bash
cd demo
dotnet restore
dotnet build
dotnet run --project Demo.WebApi
```

打开 Swagger：<http://localhost:5000/swagger>

可用示例接口：

| 接口 | 说明 |
| --- | --- |
| `GET /api/health` | 健康检查（不依赖数据库） |
| `POST /api/auth/token` | 获取 JWT 令牌 |
| `GET /api/sample/page` | 分页查询（依赖 SAMPLE_TABLE） |
| `POST /api/sample` | 新增（演示 `[AuthorizeFilter]`） |

---

## 四、如何用它开发新业务（标准套路）

**① 建实体**（`Demo.Model/Entity`）

```csharp
[SugarTable("BIZ_ORDER")]
public class BizOrder : BaseEntity
{
    [SugarColumn(ColumnName = "ORDER_NO", Length = 50)]
    public string OrderNo { get; set; }
}
```

**② 建服务接口 + 实现**（`Demo.Interfaces/IService`、`Demo.Interfaces/Service`）

```csharp
public interface IBizOrderService : IBaseService<BizOrder>
{
    Task<PagedInfo<BizOrder>> GetPageAsync(PageQueryDto query);
}

public class BizOrderService : BaseService<BizOrder>, IBizOrderService
{
    public BizOrderService(IUnitOfWork unitOfWork) : base(unitOfWork) { }

    public Task<PagedInfo<BizOrder>> GetPageAsync(PageQueryDto query)
        => GetPagesAsync(x => !x.IsDeleted, new PageParm { PageIndex = query.PageIndex, PageSize = query.PageSize });
}
```

> `BaseService<T>` 已内置 `Add / Update / Delete / GetId / GetWhere / GetPages / Saveable ...`，
> Autofac 会按「接口名 = I + 类名」自动注册，无需手动配置。名称以 `Service` 结尾即可被扫描。

**③ 建控制器**（`Demo.WebApi/Controllers`）

```csharp
[ApiGroup(ApiGroupNames.Sample)]
[Route("api/[controller]")]
public class BizOrderController : ApiControllerBase
{
    private readonly IBizOrderService _service;
    public BizOrderController(IBizOrderService service) => _service = service;

    [HttpGet("page")]
    public async Task<JsonResult> GetPage([FromQuery] PageQueryDto query)
        => ToResponse(await _service.GetPageAsync(query));
}
```

**④ 新增 Swagger 分组**：在 `Swagger/ApiGroupNames.cs` 里加一个枚举值即可。

**⑤ 定时任务**：继承 `JobBase` 并实现 `IJob`，把 `程序集名 + 类名 + Cron` 写入 `SYS_TASKS_QZ`，
通过 `ITaskSchedulerServer.AddTaskScheduleAsync` 注册即可。

---

## 五、相对原项目的优化点

1. **框架升级**：`net5.0` → `net8.0`，包全部升级到 .NET 8 兼容版本。
2. **`Directory.Build.props`**：统一管理 `TargetFramework`、语言版本、警告抑制等，子项目不再重复声明。
3. **宿主现代化**：`Startup.cs + Program.cs` 双文件 → .NET 8 最小宿主 `Program.cs` + 分门别类的
   `AddXxxSetup()` 扩展方法，可读性更强。
4. **移除静态依赖**：仓储/服务基类改为**构造函数注入 `ISqlSugarClient`**，去掉 `SqlSugar.IOC` 的
   `DbScoped` 静态调用，便于单元测试；`UnitOfWork` 直接持有 `ISqlSugarClient`，修掉了原先
   `as SqlSugarClient` 在多租户 `SqlSugarScope` 下的转换隐患。
5. **基类补充异步方法**：`IBaseService<T>` / `BaseService<T>` 增加 `AddAsync / UpdateAsync /
   DeleteAsync / GetPagesAsync / AnyAsync / CountAsync` 等，适配异步链路。
6. **数据库面向 SQL Server**：默认 `DBType = 1`，`ConnectionEncrypt` 开关控制连接串是否解密，
   并提供可直接使用的连接串模板。
7. **配置开关化**：Redis 可通过 `RedisConfig:Enabled` 一键关闭；CORS 来源配置化。
8. **清理业务代码**：删除所有业务实体/服务（用友通、短信、阿里云、Nacos、RabbitMQ 等），
   仅保留 3 个示例（`HealthController` / `AuthController` / `SampleController` + `SampleJob`）。
9. **AOP 示例落地**：Autofac 接口拦截器 + `LogAopInterceptor`，可直接扩展到日志/事务/权限切面。

---

## 六、改造成你自己的项目

全局替换前缀 `Demo` 即可：

```bash
# 1. 关闭 IDE 后执行（Windows Git Bash）
grep -rl "Demo" --include="*.cs" --include="*.csproj" --include="*.json" --include="*.sln" . \
  | xargs sed -i 's/Demo/YourCompany/g'
```

再重命名目录 `Demo.*` → `YourCompany.*`，并同步修改 `.sln` 中的项目路径即可。

---

## 七、注意事项

- `appsettings.json` 里的数据库密码、`JwtSetting:IssuerSigningKey` 等敏感信息请勿提交到仓库，
  已在 `.gitignore` 中忽略 `appsettings.Local.json` / `appsettings.Production.json`。
- Swagger 仅在 `Development` 环境开启（见 `Program.cs`）。
- 上传目录运行时自动创建为 `{程序目录}/UpLoad`，映射到 `/UpLoad`。
