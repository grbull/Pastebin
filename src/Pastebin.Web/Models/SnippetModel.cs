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

        [Display(Name = "Private")]
        public bool IsPrivate { get; set; }

        public string Content { get; set; }

        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Date Expires")]
        [DisplayFormat(NullDisplayText = "Never")]
        public DateTime? DateExpires { get; set; }
    }
}