using RGroupConstruction.Application.Common;
using MediatR;

namespace RGroupConstruction.Application.Interfaces.Query;

public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }
