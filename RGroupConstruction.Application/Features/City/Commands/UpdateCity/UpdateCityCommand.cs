using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.City.Commands.UpdateCity;

public record UpdateCityCommand : ICommand<CityDto>
{
    public string? Id { get; set; }
    public string? Name { get; set; }
}

