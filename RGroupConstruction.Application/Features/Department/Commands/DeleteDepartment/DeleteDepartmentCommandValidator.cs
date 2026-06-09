using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Department.Commands.DeleteDepartment;

internal class DeleteDepartmentCommandValidator(IDepartmentService _departmentService) : ICommandHandler<DeleteDepartmentCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
        => await _departmentService.DeleteDepartmentAsync(request, cancellationToken);
}



