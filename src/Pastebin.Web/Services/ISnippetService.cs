using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pastebin.Web.Models;

namespace Pastebin.Web.Services
{
    public interface ISnippetService
    {
        Task<SnippetModel> CreateSnippetAsync(SnippetCreateModel snippetCreateModel);
        Task<SnippetModel> GetSnippetById(Guid snippetId);
        Task<List<SnippetModel>> GetRecentSnippets(int count);
    }
}