#nullable enable
using System.ComponentModel.DataAnnotations;

namespace Pastebin.Web.Models
{
    public class SnippetCreateModel
    {
        [StringLength(50)]
        public string? Title { get; set; }

        [StringLength(10)]
        public string? Language { get; set; }

        public bool IsPrivate { get; set; }

        [StringLength(5000)]
        public string Content { get; set; }

        [Range(0, 1440)]
        public int? ExpiresInMin { get; set; }
    }
}