using Ardalis.GuardClauses;

namespace Core.Domain.Entities;

public record Entity()
{
    public required Guid Guid { get; set => field = Guard.Against.NullOrEmpty(value); }
}