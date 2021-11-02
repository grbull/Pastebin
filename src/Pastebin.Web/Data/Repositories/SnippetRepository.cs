using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pastebin.Web.Data.Entities;

namespace Pastebin.Web.Data.Repositories
{
    public class SnippetRepository : ISnippetRepository
    {
        private readonly PastebinContext _pastebinContext;
        
        public SnippetRepository(PastebinContext pastebinContext)
        {
            _pastebinContext = pastebinContext;
        }

        public async Task<Snippet> AddAsync(Snippet snippet)
        {
            snippet.Id = snippet.Id == Guid.Empty ? Guid.NewGuid() : snippet.Id;
            _pastebinContext.Add(snippet);
            await _pastebinContext.SaveChangesAsync();
            return snippet;
        }

        public async Task<Snippet> FindAsync(Guid id)
        {
            return await _pastebinContext.Snippets.FindAsync(id);
        }

        public IQueryable<Snippet> Get()
        {
            return _pastebinContext.Snippets.AsQueryable();
        }
    }
}