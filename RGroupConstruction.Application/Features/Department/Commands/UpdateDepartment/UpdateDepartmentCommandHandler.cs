using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Department.Commands.UpdateDepartment;

internal class UpdateDepartmentCommandHAndler(IDepartmentService _departmentService) : ICommandHandler<UpdateDepartmentCommand, DepartmentDto>
{
    public async Task<Result<DepartmentDto>> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
        => await _departmentService.UpdateDepartmentAsync(request, cancellationToken);
}



