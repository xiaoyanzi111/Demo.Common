using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Demo.WebApi.Swagger
{
    /// <summary>
    /// 约定：把 Controller 上的 [ApiGroup] 映射为 Swagger 的 ApiExplorer.GroupName
    /// </summary>
    public class ApiGroupConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var attr = controller.Attributes.OfType<ApiGroupAttribute>().FirstOrDefault();
            if (attr == null || attr.GroupName == null || attr.GroupName.Length == 0) return;

            controller.ApiExplorer.GroupName = attr.GroupName.First().ToString();
            controller.ApiExplorer.IsVisible = true;
        }
    }
}
