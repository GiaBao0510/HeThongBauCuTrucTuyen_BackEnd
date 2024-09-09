using AutoMapper;
using BackEnd.src.web_api.DTOs;
using BackEnd.src.core.Entities;

namespace BackEnd.src.web_api.Mappings
{
    public class MappingProfile: Profile
    {
        //hàm khởi tạo
        public MappingProfile(){
            // User - UserDto
            CreateMap<Users, UserDto>()
                .ForMember(des => des.TaiKhoan , opt => opt.Ignore()) //Bỏ qua mapping cho trường này
                .ForMember(des => des.MatKhau, opt => opt.Ignore());  //Bỏ qua việc mapping ho trường này
            CreateMap<Users, UserDto>()
                .ForMember(des => des.ID_user, opt => opt.MapFrom(src => src.ID_user))
                .ForMember(des => des.HoTen, opt => opt.MapFrom(src => src.HoTen))
                .ForMember(des => des.GioiTinh, opt => opt.MapFrom(src => src.GioiTinh))
                .ForMember(des => des.NgaySinh, opt => opt.MapFrom(src => Convert.ToDateTime(src.NgaySinh)))
                .ForMember(des => des.DiaChiLienLac, opt => opt.MapFrom(src => src.DiaChiLienLac))
                .ForMember(des => des.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(des => des.CCCD, opt => opt.MapFrom(src => src.CCCD))
                .ForMember(des => des.SDT, opt => opt.MapFrom(src => src.SDT))
                .ForMember(des => des.HinhAnh, opt => opt.MapFrom(src => src.HinhAnh))
                .ForMember(des => des.PublicID, opt => opt.MapFrom(src => src.PublicID))
                .ForMember(des => des.ID_DanToc, opt => opt.MapFrom(src => src.ID_DanToc))
                .ForMember(des => des.RoleID, opt => opt.MapFrom(src => src.RoleID));
            CreateMap<UserDto, Users>()
                .ForMember(dest => dest.NgaySinh ,opt => opt.MapFrom(src => DateTime.Parse(src.NgaySinh)))
                .ForMember(dest => dest.ID_user ,opt => opt.Ignore());
        }
    }
}