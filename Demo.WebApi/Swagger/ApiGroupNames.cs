namespace Demo.WebApi.Swagger
{
    /// <summary>
    /// 系统接口分组枚举（新增分组在此登记即可，Swagger 会自动生成对应文档）
    /// </summary>
    public enum ApiGroupNames
    {
        [GroupInfo(Title = "测试接口", Description = "用于验证框架是否正常的示例接口")]
        Test,

        [GroupInfo(Title = "登录认证", Description = "登录、令牌相关接口", Version = "v1")]
        Login,

        [GroupInfo(Title = "示例模块", Description = "分层调用链示例接口", Version = "v1")]
        Sample,
    }
}
