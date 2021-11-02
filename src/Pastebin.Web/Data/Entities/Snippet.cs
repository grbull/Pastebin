using System;

namespace Pastebin.Web.Data.Entities
{
    public class Snippet
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