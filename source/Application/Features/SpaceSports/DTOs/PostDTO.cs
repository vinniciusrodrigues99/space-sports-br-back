namespace FSP.Api.Application.Features.SpaceSports.DTOs
{
    public class PostDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Excerpt { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? CoverUrl { get; set; }
        public string Category { get; set; } = string.Empty;
        public string AuthorId { get; set; } = string.Empty;
        public PostAuthorDTO Author { get; set; } = new();
        public DateTimeOffset PublishedAt { get; set; }
        public int ReadingMinutes { get; set; }
    }

    public class PostAuthorDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
    }
}
