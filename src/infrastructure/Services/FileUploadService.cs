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

            //Kiểm tra xem hoạt động có yêu cầu truy cập dữ liệu dạng "multipart/form-data" hay không, nếu không thì trả về 
            if (operation.RequestBody == null || !operation.RequestBody.Content.Any(x => x.Key.Equals(fileUploadMime, StringComparison.InvariantCultureIgnoreCase)))
                return;

            //Lấy danh sách các tham số từ phương thức. Để lọc ra các phương thức có tham số kiểu IFormFile(để xác định xem tham số nào liên quan đến việc upload file)
            var fileParams = context.MethodInfo.GetParameters().Where(p => p.ParameterType == typeof(IFormFile));

            //Cập nhật lại sơ đồ của RequestBody, với các tham số liên quan đến việc upload file, cập nhật schema của request bidy để phản ánh việc upload file
            //Mỗi tham số sẽ được thêm vào schema với Type ="string" và Format là "binary" (Đại diện cho việc truyền dưới dạng binary)
            operation.RequestBody.Content[fileUploadMime].Schema.Properties =
                fileParams.ToDictionary(k => k.Name, v => new OpenApiSchema()
                {
                    Type = "string",
                    Format = "binary"
                });
        }
    }
}