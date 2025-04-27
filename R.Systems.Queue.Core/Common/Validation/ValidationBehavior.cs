using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace R.Systems.Queue.Core.Common.Validation;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (!_validators.Any())
        {
            return await next(cancellationToken);
        }

        List<ValidationFailure> validationFailures = [];
        ValidationContext<TRequest> context = new(request);
        foreach (IValidator<TRequest> validator in _validators)
        {
            ValidationResult result = await validator.ValidateAsync(context, cancellationToken);
            if (result.Errors.Count == 0)
            {
                continue;
            }

            validationFailures.AddRange(result.Errors);
        }

        if (validationFailures.Count > 0)
        {
            throw new ValidationException(validationFailures);
        }

        return await next(cancellationToken);
    }
}
