using FluentResults;

namespace Core.Application.Errors;

public class NotFoundError() : Error(Message)
{
    public new const string Message = "Not found error";
}