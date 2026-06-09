using RGroupConstruction.Application.Common;
using MediatR;

namespace RGroupConstruction.Application.Interfaces.Query;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse> { }
