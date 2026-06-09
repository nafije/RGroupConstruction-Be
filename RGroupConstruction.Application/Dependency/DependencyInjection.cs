using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Services;
using CarRentalZaimi.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace RGroupConstruction.Application.Dependency;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationCommon(this IServiceCollection services)
    {
        services.AddScoped<IErrorService, ErrorService>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IUnitService, UnitService>();
        services.AddScoped<ILayoutService, LayoutService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IJobService, JObService>();
        services.AddScoped<IJobApplicationService, JobApplicationService>(); 
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ISubscribeService, SubscribeService>();
        services.AddScoped<IAdsService, AdsService>();
        services.AddScoped<IStatusService, StatusService>();
        services.AddScoped<IContactMessageService, ContactMessageService>();
        services.AddScoped<ICompanyInfoService, CompanyInfoService>();
        services.AddScoped<IParkingService, ParkingService>();
        services.AddScoped<ILogService, LogService>();
        services.AddScoped<IUnitClientService, UnitClientService>();
        services.AddScoped<ICityService, CityService>();
        return services;
    }
}

