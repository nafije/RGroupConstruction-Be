using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;

namespace RGroupConstruction.Application.Features.Department.Queries.GetAllDepartments;

public class GetAllDepartmentsQuery : IQuery<IEnumerable<DepartmentDto>>;

