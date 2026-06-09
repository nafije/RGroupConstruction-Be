using RGroupConstruction.Application.Common;
using FluentValidation;
using MediatR;

namespace RGroupConstruction.Application.Behavior;


public class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .ToList();

        if (failures.Count == 0)
            return await next(cancellationToken);

        var errors = new ValidationErrorCollection();
        foreach (var failure in failures)
            errors.Add(failure.PropertyName, failure.ErrorMessage);

        return CreateValidationResponse(errors);
    }

    private static TResponse CreateValidationResponse(ValidationErrorCollection errors)
    {
        if (typeof(TResponse) == typeof(Result))
            return (TResponse)(object)Result.ValidationError(errors);

        if (typeof(TResponse).IsGenericType &&
            typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var resultType = typeof(TResponse).GetGenericArguments()[0];
            var method = typeof(Result<>)
                .MakeGenericType(resultType)
                .GetMethod(nameof(Result<object>.ValidationError))!;

            return (TResponse)method.Invoke(null, new object[] { errors })!;
        }

        throw new ValidationException(errors.Select(error =>
            new FluentValidation.Results.ValidationFailure(error.PropertyName, error.ErrorMessage)));
    }
}

