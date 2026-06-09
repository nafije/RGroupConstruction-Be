using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.City.Commands.CreateCity;

public class CreateCityCommand : ICommand<CityDto>
{
    public string? Name { get; set; }
}

