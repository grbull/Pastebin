#nullable enable
using System;
using System.ComponentModel.DataAnnotations;

namespace Pastebin.Web.Models
{
    public class SnippetModel
    {
        public Guid Id { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "Untitled")]
        public string? Title { get; set; }
        
        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "None")]
        public string? Language { get; set; }

        public bool IsPrivate { get; set; }

        public string Content { get; set; }

        public DateTime DateCreated { get; set; }

        [DisplayFormat(NullDisplayText = "Never")]
        public DateTime? DateExpires { get; set; }
    }
}