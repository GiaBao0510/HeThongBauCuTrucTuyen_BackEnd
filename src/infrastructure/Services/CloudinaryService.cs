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
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);
            return result;
        }

        //Đưa ảnh lên
        public async Task<ImageUploadResult> UploadImage(IFormFile file){
            var uploadResult = new ImageUploadResult();

            if(file.Length>0){
                using(var stream = file.OpenReadStream()){
                    var uploadParams = new ImageUploadParams(){
                        File = new FileDescription(file.FileName, stream),
                        Folder="NguoiDung"             //Thư mục lưu trữ
                    };
                    uploadResult = await _cloudinary.UploadAsync(uploadParams);
                }
            }
            
            return uploadResult;  
        }

        //Thay đổi ảnh đựa trên PublicId
        public async Task<ImageUploadResult> UpdateImage(string publicId, IFormFile file){
            //Xóa ảnh cũ
            await DeleteImage(publicId);

            //Cập nhật ảnh mới
            var result = await UploadImage(file);
            return result;
        }
    }
}