using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace BackEnd.src.infrastructure.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;
      
        //Khởi tạo
        public  CloudinaryService(IConfiguration configuration){
            var account = new Account(
                configuration["CloudinarySetting:CloudName"],
                configuration["CloudinarySetting:ApiKey"],
                configuration["CloudinarySetting:ApiSecret"]
            );
            _cloudinary = new Cloudinary(account);
        }

        //Xóa hình ảnh dựa trên publicID của hình ảnh trên cloudinary
        public async Task<DeletionResult> DeleteImage(string publicId){
            try{
                var deleteParams = new DeletionParams(publicId);
                return await _cloudinary.DestroyAsync(deleteParams);
            }catch(Exception ex){
                throw new Exception(ex.Message);
            }
        }

        //Đưa ảnh lên
        public async Task<ImageUploadResult> UploadImage(IFormFile file){
            if (file.Length == 0){
                throw new ArgumentException("File is empty", nameof(file));
            }

            try{
                var uploadResult = new ImageUploadResult();

                using(var stream = file.OpenReadStream()){
                    var uploadParams = new ImageUploadParams(){
                        File = new FileDescription(file.FileName, stream),
                        Folder="NguoiDung",             //Thư mục lưu trữ
                        Transformation = new Transformation()
                                            .Quality("auto")    //Chất lượng ảnh
                                            .FetchFormat("auto") //Định dạng ảnh
                                            .Flags("preserve_transparency") //Đảm bảo ảnh không mất màu
                    };
                    return await _cloudinary.UploadAsync(uploadParams);
                }
            }catch(Exception ex){
                throw new Exception(ex.Message);
            }
        }

        //Thay đổi ảnh đựa trên PublicId
        public async Task<ImageUploadResult> UpdateImage(string publicId, IFormFile file){
            try{
                //Xóa ảnh cũ
                await DeleteImage(publicId);

                //Cập nhật ảnh mới
                return await UploadImage(file);
            }catch(Exception ex){
                throw new Exception(ex.Message);
            }
        }
    }
}