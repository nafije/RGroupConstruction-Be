using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.Log.Queries.GetLogById;

public class GetLogByIdQuery(int? id) : ICommand<LogEntryDto>
{
    public int? Id { get; } = id;
}

