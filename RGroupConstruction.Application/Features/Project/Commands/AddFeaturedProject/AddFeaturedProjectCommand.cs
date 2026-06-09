using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.Project.Commands.AddFeaturedProject;

public class AddFeaturedProjectCommand : ICommand<bool>
{
    public string? ProjectId { get; set; }
    public bool IsFeatured{ get; set; }
}
