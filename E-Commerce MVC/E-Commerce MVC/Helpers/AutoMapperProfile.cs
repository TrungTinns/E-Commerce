using AutoMapper;
using E_Commerce_MVC.Data;
using E_Commerce_MVC.ViewModels;

namespace E_Commerce_MVC.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() 
        {
            CreateMap<SignUpVM, KhachHang>()
                .ForMember(user => user.HoTen, opt => opt.MapFrom(SignUpVM => SignUpVM.FullName))
                .ForMember(user => user.GioiTinh, opt => opt.MapFrom(SignUpVM => SignUpVM.Gender))
                .ForMember(user => user.NgaySinh, opt => opt.MapFrom(SignUpVM => SignUpVM.BirthDate ?? default(DateTime)))
                .ForMember(user => user.DiaChi, opt => opt.MapFrom(SignUpVM => SignUpVM.Address))
                .ForMember(user => user.DienThoai, opt => opt.MapFrom(SignUpVM => SignUpVM.Phone))
                .ForMember(user => user.Email, opt => opt.MapFrom(SignUpVM => SignUpVM.Email))
                .ForMember(user => user.Hinh, opt => opt.MapFrom(SignUpVM => SignUpVM.Image))
                .ForMember(user => user.MatKhau, opt => opt.Ignore());
        }
    }
}
