using FluentResults;

namespace Core.Application.Errors;

public class ValidationError : Error
{
    public new const string Message = "Validation error";

    public ValidationError(string propertyName, string errorMessage) : base($"{Message}: {errorMessage}")
    {
        Metadata.Add(propertyName, errorMessage);
    }
}