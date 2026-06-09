using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Department.Commands.CreateDepartment;

public class CreateDepartmentCommandHandler(IDepartmentService _departmentService) : ICommandHandler<CreateDepartmentCommand, DepartmentDto>
{
    public async Task<Result<DepartmentDto>> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        => await _departmentService.CreateDepartmentAsync(request, cancellationToken);
}


