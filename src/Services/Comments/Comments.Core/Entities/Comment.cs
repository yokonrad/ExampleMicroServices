using Ardalis.GuardClauses;
using Core.Domain.Entities;

namespace Comments.Core.Entities;

public record Comment() : Entity
{
    public required Guid PostGuid { get; set => field = Guard.Against.NullOrEmpty(value); }
    public required string Text { get; set => field = Guard.Against.NullOrWhiteSpace(value); }
    public required bool Visible { get; set; }
}