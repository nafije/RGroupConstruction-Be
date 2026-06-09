using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.ContactMessage.Commands.CreateContactMessage;

internal class CreateContactMessageCommandHandler(IContactMessageService _contactMessageService) : ICommandHandler<CreateContactMessageCommand, bool>
{
    public async Task<Result<bool>> Handle(CreateContactMessageCommand request, CancellationToken cancellationToken)
        => await _contactMessageService.CreateAsync(request, cancellationToken);
}

