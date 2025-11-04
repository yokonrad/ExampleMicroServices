using Core.Application.Errors;
using FluentResults;

namespace Core.Application.Extensions;

public static class FluentResultsExtension
{
    public static Dictionary<string, string[]> GetValidationErrors<T>(this Result<T> result)
    {
        var dictionary = new Dictionary<string, string[]>();

        foreach (var error in result.Errors)
        {
            if (!error.Message.Contains(ValidationError.Message)) continue;

            foreach (var metadata in error.Metadata)
            {
                if (metadata.Value is not string value) continue;

                if (dictionary.TryGetValue(metadata.Key, out var existingErrors)) dictionary[metadata.Key] = [.. existingErrors, value];
                else dictionary.Add(metadata.Key, [value]);
            }
        }

        return dictionary;
    }
}