using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pastebin.Web.Models;

namespace Pastebin.Web.Services
{
    public interface ISnippetService
    {
        Task<SnippetModel> CreateAsync(SnippetCreateModel snippetCreateModel);
        Task<SnippetModel> GetByIdAsync(Guid snippetId);
        Task<List<SnippetModel>> GetRecentAsync(int count);
    }
}