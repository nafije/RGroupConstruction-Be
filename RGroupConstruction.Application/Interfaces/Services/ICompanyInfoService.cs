using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.CompanyInfo.Commands.AddCompanyInfo;
using RGroupConstruction.Application.Features.CompanyInfo.Queries.GetCompanyInfo;

namespace RGroupConstruction.Application.Interfaces.Services;

public interface ICompanyInfoService
{
    Task<Result<CompanyInfoDto>> AddCompanyInfoDataAsync(AddCompanyInfoCommand request, CancellationToken cancellationToken = default);
    Task<Result<CompanyInfoDto>> GetCompanyProfileDataAsync(GetCompanyInfoQuery request, CancellationToken cancellationToken = default);
}

