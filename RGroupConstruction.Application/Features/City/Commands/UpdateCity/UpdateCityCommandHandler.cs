using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.City.Commands.UpdateCity;

internal class UpdateCityCommandHandler(ICityService _cityService) : ICommandHandler<UpdateCityCommand, CityDto>
{
    public async Task<Result<CityDto>> Handle(UpdateCityCommand request, CancellationToken cancellationToken)
        => await _cityService.UpdateCityAsync(request, cancellationToken);
}

