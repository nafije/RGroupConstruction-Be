using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.City.Commands.DeleteCity;

public class DeleteCityCommand(string? id) : ICommand<bool>
{
    public string? Id { get; } = id;
}


