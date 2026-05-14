namespace FSP.Api.Application.Features.SpaceSports.Posts.Commands.UpdatePost
{
    public class UpdatePostRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Excerpt { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? CoverUrl { get; set; }
        public string Category { get; set; } = string.Empty;
    }
}
