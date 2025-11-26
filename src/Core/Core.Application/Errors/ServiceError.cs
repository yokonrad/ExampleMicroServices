using FluentResults;

namespace Core.Application.Errors;

public class ServiceError() : Error(Message)
{
    public new const string Message = "Service error";
}