using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Domain.Entities;
using AutoMapper;

namespace RGroupConstruction.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Ads, AdsDto>().ReverseMap();
        CreateMap<Job, JobDto>().ReverseMap();
        CreateMap<JobApplication, JobApplicationDto>().ReverseMap();
        CreateMap<CompanyInfo, CompanyInfoDto>().ReverseMap();
        CreateMap<Layout, LayoutDto>().ReverseMap();
        CreateMap<Project, ProjectDto>()
            .ForMember(
                d => d.ProjectImages,
                opt => opt.MapFrom(
                    s => s.ProjectImages!.Where(i => !i.IsDeleted)
                )
            )
            .ForMember(
                d => d.ProjectTranslations,
                opt => opt.MapFrom(
                    s => s.ProjectTranslations!.Where(t => !t.IsDeleted)
                )
            )
            .ReverseMap();
        CreateMap<ProjectImage, ProjectImageDto>().ReverseMap();
        CreateMap<RefreshToken, RefreshTokenDto>().ReverseMap();
        CreateMap<Role, RoleDto>().ReverseMap();
        CreateMap<Subscribe, SubscribeDto>().ReverseMap();
        CreateMap<Unit, UnitDto>().ReverseMap();
        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<UnitImage, UnitImageDto>().ReverseMap();
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<UserNotification, UserNotificationDto>().ReverseMap();
        CreateMap<ProjectTranslation, ProjectTranslationDto>().ReverseMap();
        CreateMap<AdsTranslation, AdsTranslationDto>().ReverseMap();
        CreateMap<CompanyInfoTranslation, CompanyInfoTranslationDto>().ReverseMap();
        CreateMap<Department, DepartmentDto>().ReverseMap();
        CreateMap<Status, StatusDto>().ReverseMap();
        CreateMap<Parking, ParkingDto>().ReverseMap();
        CreateMap<UnitClient, UnitClientDto>().ReverseMap();
        CreateMap<City, CityDto>().ReverseMap();

    }
}

