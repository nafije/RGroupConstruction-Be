using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.Features.ContactMessage.Commands.CreateContactMessage;

namespace RGroupConstruction.Application.Interfaces.Services;

public interface IContactMessageService
{
    Task<Result<bool>> CreateAsync(CreateContactMessageCommand request, CancellationToken cancellationToken = default);
}

