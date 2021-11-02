namespace Pastebin.Web.Models
{
    public class SnippetCreateModel
    {
        public string? Title { get; set; }

        public string? Language { get; set; }

        public bool IsPrivate { get; set; }

        public string Content { get; set; }

        public int? ExpiresInMin { get; set; }
    }
}