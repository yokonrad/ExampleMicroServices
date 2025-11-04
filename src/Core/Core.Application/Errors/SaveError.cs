using FluentResults;

namespace Core.Application.Errors;

public class SaveError() : Error(Message)
{
    public new const string Message = "Save error";
}