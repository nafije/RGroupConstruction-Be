using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.CompanyInfo.Commands.AddCompanyInfo;

public class AddCompanyInfoCommand : ICommand<CompanyInfoDto>
{
    public string? Name { get; set; }
    public string? Tagline { get; set; }
    public string? AboutText { get; set; }
    public string? MissionTitle { get; set; }
    public string? MissionDescription { get; set; }
    public string? WhyChooseUs { get; set; } 
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? SalesPhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? WorkingHours { get; set; } 
    public string? FacebookUrl { get; set; }
    public string? InstagramUrl { get; set; }
    public string? TwiterUrl { get; set; }
    public string? YoutubeUrl { get; set; }
    public int Years { get; set; }
    public int Projects { get; set; }
    public int Clients { get; set; }
    public List<CompanyInfoTranslationDto> CompanyInfoTranslations { get; set; } = [];
}

