#nullable enable
using System;

namespace Pastebin.Web.Models
{
    public class SnippetModel
    {
        public Guid Id { get; set; }

        public string? Title { get; set; }

        public string? Language { get; set; }

        public bool IsPrivate { get; set; }

        public string Content { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DateExpires { get; set; }
    }
}