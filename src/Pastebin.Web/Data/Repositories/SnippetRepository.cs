using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<Snippet>> GetRecentAsync(int count)
        {
            var recentSnippets = await _pastebinContext.Snippets.AsQueryable()
                .Where(s => s.IsPrivate == false)
                .Where(s => s.DateExpires == null || s.DateExpires > DateTime.UtcNow)
                .OrderByDescending(s => s.DateCreated)
                .Take(count)
                .ToListAsync();

            return recentSnippets;
        }
    }
}