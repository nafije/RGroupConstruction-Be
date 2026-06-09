using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.CompanyInfo.Commands.AddCompanyInfo;

internal class AddCompanyInfoCommandHandler(ICompanyInfoService _companyInfoService) : ICommandHandler<AddCompanyInfoCommand, CompanyInfoDto>
{
    public async Task<Result<CompanyInfoDto>> Handle(AddCompanyInfoCommand request, CancellationToken cancellationToken)
        => await _companyInfoService.AddCompanyInfoDataAsync(request, cancellationToken);
}


