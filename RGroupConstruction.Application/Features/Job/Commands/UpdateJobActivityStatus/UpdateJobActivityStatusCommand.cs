using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.Job.Commands.UpdateJobActivityStatus;

public class UpdateJobActivityStatusCommand : ICommand<bool>
{
    public string? JobId { get; set; }
    public bool IsActive { get; set; }
}
