using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.Department.Commands.CreateDepartment;
using RGroupConstruction.Application.Features.Department.Commands.DeleteDepartment;
using RGroupConstruction.Application.Features.Department.Commands.UpdateDepartment;
using RGroupConstruction.Application.Features.Department.Queries.GetAllDepartments;
using RGroupConstruction.Application.Features.Department.Queries.GetAllPagedDepartments;

namespace RGroupConstruction.Application.Interfaces.Services;

public interface IDepartmentService
{
    Task<Result<DepartmentDto>> CreateDepartmentAsync(CreateDepartmentCommand request, CancellationToken cancellationToken = default);
    Task<Result<DepartmentDto>> UpdateDepartmentAsync(UpdateDepartmentCommand request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteDepartmentAsync(DeleteDepartmentCommand request, CancellationToken cancellationToken = default);
    Task<Result<PagedResponse<DepartmentDto>>> GetAllPagedDepartmentsAsync(GetAllPagedDepartmentsQuery request, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<DepartmentDto>>> GetAllDepartmentsAsync(GetAllDepartmentsQuery request, CancellationToken cancellationToken = default);
}

