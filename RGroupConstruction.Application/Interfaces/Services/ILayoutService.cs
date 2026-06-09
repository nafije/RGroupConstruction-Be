using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.Layout.Commands.CreateLayout;
using RGroupConstruction.Application.Features.Layout.Commands.DeleteLayout;
using RGroupConstruction.Application.Features.Layout.Commands.UpdateLayout;
using RGroupConstruction.Application.Features.Layout.Queries.GetAllLayouts;
using RGroupConstruction.Application.Features.Layout.Queries.GetAllPagedLayouts;

namespace RGroupConstruction.Application.Interfaces.Services;

public interface ILayoutService
{
    Task<Result<LayoutDto>> CreateLayoutAsync(CreateLayoutCommand request, CancellationToken cancellationToken = default);
    Task<Result<LayoutDto>> UpdateLayoutAsync(UpdateLayoutCommand request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteLayoutAsync(DeleteLayoutCommand request, CancellationToken cancellationToken = default);
    Task<Result<PagedResponse<LayoutDto>>> GetAllPagedLayoutsAsync(GetAllPagedLayoutsQuery request, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<LayoutDto>>> GetAllLayoutsAsync(GetAllLayoutsQuery request, CancellationToken cancellationToken = default);
}

