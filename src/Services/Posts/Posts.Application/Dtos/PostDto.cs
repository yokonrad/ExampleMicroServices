namespace Posts.Application.Dtos;

public record PostDto()
{
    public required Guid Guid { get; init; }
    public required string Title { get; init; }
    public required string Text { get; init; }
    public required bool Visible { get; init; }
}