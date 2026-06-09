using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.City.Commands.CreateCity;

internal class CreateCityCommandHandler(ICityService _cityService) : ICommandHandler<CreateCityCommand, CityDto>
{
    public async Task<Result<CityDto>> Handle(CreateCityCommand request, CancellationToken cancellationToken)
        => await _cityService.CreateCityAsync(request, cancellationToken);
}

