using RGroupConstruction.Application.Common;
using MediatR;

namespace RGroupConstruction.Application.Interfaces.Command;


public interface ICommand<TData> : IRequest<Result<TData>> { }

public interface ICommandHandler<TCommand, TData>
    : IRequestHandler<TCommand, Result<TData>>
    where TCommand : ICommand<TData>
{ }
