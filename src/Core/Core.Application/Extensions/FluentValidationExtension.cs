using Core.Application.Errors;
using FluentResults;
using FluentValidation.Results;

namespace Core.Application.Extensions;

public static class FluentValidationExtension
{
    public static IError[] GetValidationErrors(this ValidationResult validationResult)
    {
        return [.. validationResult.Errors.Select(x => new ValidationError(x.PropertyName.ToCamelCase(), x.ErrorMessage))];
    }
}