using System.Text.Json;

namespace Core.Application.Extensions;

public static class StringExtension
{
    public static string ToCamelCase(this string value)
    {
        return JsonNamingPolicy.CamelCase.ConvertName(value);
    }
}