using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.CompanyInfo.Queries.GetCompanyInfo;

internal class GetCompanyInfoQueryHandler(ICompanyInfoService _companyInfoService) : IQueryHandler<GetCompanyInfoQuery, CompanyInfoDto>
{
    public async Task<Result<CompanyInfoDto>> Handle(GetCompanyInfoQuery request, CancellationToken cancellationToken)
        => await _companyInfoService.GetCompanyProfileDataAsync(request, cancellationToken);
}
