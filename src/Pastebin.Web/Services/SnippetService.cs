using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pastebin.Web.Data.Entities;
using Pastebin.Web.Data.Repositories;
using Pastebin.Web.Models;

namespace Pastebin.Web.Services
{
    public class SnippetService : ISnippetService
    {
        private readonly ISnippetRepository _snippetRepository;

        public SnippetService(ISnippetRepository snippetRepository)
        {
            _snippetRepository = snippetRepository;
        }

        public async Task<SnippetModel> CreateAsync(SnippetCreateModel snippetCreateModel)
        {
            if (snippetCreateModel is null)
            {
                throw new ArgumentNullException(nameof(snippetCreateModel));
            }

            var dateCreated = DateTime.UtcNow;
            var snippetEntity = new Snippet
            {
                Id = Guid.NewGuid(),
                Title = snippetCreateModel.Title,
                Language = snippetCreateModel.Language,
                IsPrivate = snippetCreateModel.IsPrivate,
                Content = snippetCreateModel.Content,
                DateCreated = dateCreated,
                DateExpires = snippetCreateModel.ExpiresInMin.HasValue
                    ? dateCreated.AddMinutes((int) snippetCreateModel.ExpiresInMin)
                    : null
            };

            snippetEntity = await _snippetRepository.AddAsync(snippetEntity);

            return new SnippetModel
            {
                Id = snippetEntity.Id,
                Title = snippetEntity.Title,
                Language = snippetEntity.Language,
                IsPrivate = snippetEntity.IsPrivate,
                Content = snippetEntity.Content,
                DateCreated = snippetEntity.DateCreated,
                DateExpires = snippetEntity.DateExpires
            };
        }

        public async Task<SnippetModel> GetByIdAsync(Guid snippetId)
        {
            var snippetEntity = await _snippetRepository.FindAsync(snippetId);

            if (snippetEntity is null)
            {
                return null;
            }

            if (snippetEntity.DateExpires.HasValue && DateTime.UtcNow > snippetEntity.DateExpires)
            {
                return null;
            }

            return new SnippetModel
            {
                Id = snippetEntity.Id,
                Title = snippetEntity.Title,
                Language = snippetEntity.Language,
                IsPrivate = snippetEntity.IsPrivate,
                Content = snippetEntity.Content,
                DateCreated = snippetEntity.DateCreated,
                DateExpires = snippetEntity.DateExpires
            };
        }

        public async Task<List<SnippetModel>> GetRecentAsync(int count)
        {
            var snippets = await _snippetRepository.GetRecentAsync(count);

            return snippets.Select(snippet => new SnippetModel
            {
                Id = snippet.Id,
                Title = snippet.Title,
                Language = snippet.Language,
                IsPrivate = snippet.IsPrivate,
                Content = snippet.Content,
                DateCreated = snippet.DateCreated,
                DateExpires = snippet.DateExpires
            }).ToList();
        }
    }
}