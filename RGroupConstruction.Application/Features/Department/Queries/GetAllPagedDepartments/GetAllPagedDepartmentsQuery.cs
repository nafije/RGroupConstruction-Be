using RGroupConstruction.Application.Common.Paging;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;

namespace RGroupConstruction.Application.Features.Department.Queries.GetAllPagedDepartments;

public class GetAllPagedDepartmentsQuery : PagedQuery, IQuery<PagedResponse<DepartmentDto>> { }


