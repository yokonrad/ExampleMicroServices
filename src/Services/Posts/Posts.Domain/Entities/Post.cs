using Ardalis.GuardClauses;
using Core.Domain.Entities;

namespace Posts.Domain.Entities;

public record Post() : Entity
{
    public required string Title { get; set => field = Guard.Against.NullOrWhiteSpace(value); }
    public required string Text { get; set => field = Guard.Against.NullOrWhiteSpace(value); }
    public required bool Visible { get; set; }
}