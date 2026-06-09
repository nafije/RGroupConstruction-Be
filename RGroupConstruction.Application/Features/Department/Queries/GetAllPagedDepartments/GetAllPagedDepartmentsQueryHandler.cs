using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Department.Queries.GetAllPagedDepartments;

internal class GetAllPagedDepartmentsQueryHandler(IDepartmentService _departmentService) : IQueryHandler<GetAllPagedDepartmentsQuery, PagedResponse<DepartmentDto>>
{
    public async Task<Result<PagedResponse<DepartmentDto>>> Handle(GetAllPagedDepartmentsQuery request, CancellationToken cancellationToken)
        => await _departmentService.GetAllPagedDepartmentsAsync(request, cancellationToken);
}


