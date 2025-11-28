namespace Comments.Core.Dtos;

public record CommentDto()
{
    public required Guid Guid { get; init; }
    public required Guid PostGuid { get; init; }
    public required string Text { get; init; }
    public required bool Visible { get; init; }
}