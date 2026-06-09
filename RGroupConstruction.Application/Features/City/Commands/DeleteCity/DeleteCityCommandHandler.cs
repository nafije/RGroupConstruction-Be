using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.City.Commands.DeleteCity;

internal class DeleteCityCommandHandler(ICityService _cityService) : ICommandHandler<DeleteCityCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteCityCommand request, CancellationToken cancellationToken)
        => await _cityService.DeleteCityAsync(request, cancellationToken);
}

