using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BackEnd.src.infrastructure.Services
{
    //Lớp này được tạo ra dùng để tùy chỉnh các hoạt động của API. Trước khi xuất ra tài liệu Swagger
    public class FileUploadService: IOperationFilter
    {
        //Phương thức này triển khai từ giao diện IOperationFilter.
        //Hàm này được gọi khi ánh xạ hoạt động trong OpenAPI document. Hàm này có thêm các tham số tượng ứng cho việc upload file
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var fileUploadMime = "multipart/form-data";
            if (operation.RequestBody == null || !operation.RequestBody.Content.Any(x => x.Key.Equals(fileUploadMime, StringComparison.InvariantCultureIgnoreCase)))
                return;

            var fileParams = context.MethodInfo.GetParameters().Where(p => p.ParameterType == typeof(IFormFile));
            operation.RequestBody.Content[fileUploadMime].Schema.Properties =
                fileParams.ToDictionary(k => k.Name, v => new OpenApiSchema()
                {
                    Type = "string",
                    Format = "binary"
                });
        }
    }
}