using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Department.Queries.GetAllDepartments;

internal class GetAllDepartmentsQueryHandler(IDepartmentService _departmentService) : IQueryHandler<GetAllDepartmentsQuery, IEnumerable<DepartmentDto>>
{
    public async Task<Result<IEnumerable<DepartmentDto>>> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken)
        => await _departmentService.GetAllDepartmentsAsync(request, cancellationToken);
}

