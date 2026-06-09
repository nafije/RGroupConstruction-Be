using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.Status.Commands.CreateStatus;
using RGroupConstruction.Application.Features.Status.Commands.DeleteStatus;
using RGroupConstruction.Application.Features.Status.Commands.UpdateStatus;
using RGroupConstruction.Application.Features.Status.Queries.GetAllPagedStatuses;
using RGroupConstruction.Application.Features.Status.Queries.GetAllStatuses;

namespace RGroupConstruction.Application.Interfaces.Services;

public interface IStatusService
{
    Task<Result<StatusDto>> CreateStatusAsync(CreateStatusCommand request, CancellationToken cancellationToken = default);
    Task<Result<StatusDto>> UpdateStatusAsync(UpdateStatusCommand request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteStatusAsync(DeleteStatusCommand request, CancellationToken cancellationToken = default);
    Task<Result<PagedResponse<StatusDto>>> GetAllPagedStatusesAsync(GetAllPagedStatusesQuery request, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<StatusDto>>> GetAllStatusesAsync(GetAllStatusesQuery request, CancellationToken cancellationToken = default);
}

