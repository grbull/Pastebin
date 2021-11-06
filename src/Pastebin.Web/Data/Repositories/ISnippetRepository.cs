using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pastebin.Web.Data.Entities;

namespace Pastebin.Web.Data.Repositories
{
    public interface ISnippetRepository
    {
        Task<Snippet> AddAsync(Snippet snippet);
        Task<Snippet> FindAsync(Guid id);
        IQueryable<Snippet> Get();
        Task<List<Snippet>> GetRecentAsync(int count);
    }
}