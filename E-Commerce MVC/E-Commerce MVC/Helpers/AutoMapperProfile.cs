using AutoMapper;
using E_Commerce_MVC.Data;
using E_Commerce_MVC.ViewModels;

namespace E_Commerce_MVC.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() 
        {
            CreateMap<SignUpVM, KhachHang>();
                /*.ForMember(user => user.HoTen, option => option.MapFrom(SignUpVM => SignUpVM.FullName)).ReverseMap();*/
        }
    }
}
