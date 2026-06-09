using RGroupConstruction.API.Controllers.Base;
using RGroupConstruction.Application.Features.CompanyInfo.Commands.AddCompanyInfo;
using RGroupConstruction.Application.Features.CompanyInfo.Queries.GetCompanyInfo;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Resources;
using RGroupConstruction.Domain.Common.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RGroupConstruction.API.Controllers;

public class CompanyInfoController(IMediator _mediator,IMessageLocalizer localizer) : ApiControllerBase(_mediator, localizer)
{
    [HttpPost("addCompanyInfoData")]
    [Authorize(SystemPolicies.Admin)]
    public async Task<IActionResult> AddCompanyProfileData([FromBody] AddCompanyInfoCommand command)
    {
        return await SendCommand(command, _localizer[MessageKeys.Success.CompanyProfile.Added]);
    }

    [HttpGet("get", Name = nameof(GetCompanyProfileData))]
    public async Task<IActionResult> GetCompanyProfileData([FromQuery] GetCompanyInfoQuery query)
    {
        return await SendQuery(query, null, StatusCodes.Status404NotFound);
    }
}
