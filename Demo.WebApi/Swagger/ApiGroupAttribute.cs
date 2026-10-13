using System;

namespace Demo.WebApi.Swagger
{
    /// <summary>
    /// 用于标注在 ApiGroupNames 枚举值上，描述分组信息
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class GroupInfoAttribute : Attribute
    {
        public string Title { get; set; }
        public string Version { get; set; } = "v1";
        public string Description { get; set; }
    }

    /// <summary>
    /// 标注在 Controller 上，指定其所属的 Swagger 分组
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ApiGroupAttribute : Attribute
    {
        public ApiGroupAttribute(params ApiGroupNames[] name)
        {
            GroupName = name;
        }

        public ApiGroupNames[] GroupName { get; set; }
    }
}
