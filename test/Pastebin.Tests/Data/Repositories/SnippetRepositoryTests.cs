using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pastebin.Web.Data;
using Pastebin.Web.Data.Entities;
using Pastebin.Web.Data.Repositories;
using Xunit;

namespace Pastebin.Tests.Data.Repositories
{
    public class SnippetRepositoryTests
    {
        private readonly DbContextOptions<PastebinContext> _contextOptions;

        public SnippetRepositoryTests()
        {
            _contextOptions = new DbContextOptionsBuilder<PastebinContext>().UseInMemoryDatabase("PastebinDB").Options;
        }

        [Fact]
        public async Task AddAsync_Can_Add_Snippet()
        {
            // Arrange
            var testSnippet = new Snippet
            {
                Id = Guid.NewGuid(),
                Title = "Test Snippet",
                Language = "c#",
                IsPrivate = false,
                Content = "Test Content",
                DateCreated = DateTime.UtcNow,
                DateExpires = DateTime.UtcNow.AddMinutes(60),
            };

            // Act
            using (var context = new PastebinContext(_contextOptions))
            {
                var repository = new SnippetRepository(context);
                await repository.AddAsync(testSnippet);
            }

            // Assert
            using (var context = new PastebinContext(_contextOptions))
            {
                var snippet = await context.Snippets.FindAsync(testSnippet.Id);

                Assert.Equal(testSnippet.Id, snippet.Id);
                Assert.Equal(testSnippet.Title, snippet.Title);
                Assert.Equal(testSnippet.Language, snippet.Language);
                Assert.Equal(testSnippet.IsPrivate, snippet.IsPrivate);
                Assert.Equal(testSnippet.Content, snippet.Content);
                Assert.Equal(testSnippet.DateCreated, snippet.DateCreated);
                Assert.Equal(testSnippet.DateExpires, snippet.DateExpires);
            }
        }

        [Fact]
        public async Task FindAsync_Can_Find_Snippet()
        {
            // Arrange
            var testSnippet = new Snippet
            {
                Id = Guid.NewGuid(),
                Title = "Test Snippet",
                Language = "c#",
                IsPrivate = false,
                Content = "Test Content",
                DateCreated = DateTime.UtcNow,
                DateExpires = DateTime.UtcNow.AddMinutes(60),
            };

            // Act
            using (var context = new PastebinContext(_contextOptions))
            {
                await context.AddAsync(testSnippet);
                await context.SaveChangesAsync();
            }

            // Assert
            using (var context = new PastebinContext(_contextOptions))
            {
                var repository = new SnippetRepository(context);
                var snippet = await repository.FindAsync(testSnippet.Id);

                Assert.Equal(testSnippet.Id, snippet.Id);
                Assert.Equal(testSnippet.Title, snippet.Title);
                Assert.Equal(testSnippet.Language, snippet.Language);
                Assert.Equal(testSnippet.IsPrivate, snippet.IsPrivate);
                Assert.Equal(testSnippet.Content, snippet.Content);
                Assert.Equal(testSnippet.DateCreated, snippet.DateCreated);
                Assert.Equal(testSnippet.DateExpires, snippet.DateExpires);
            }
        }

        [Fact]
        public async Task Get_Returns_Valid_IQueryable()
        {
            // Arrange
            var testSnippet = new Snippet
            {
                Id = Guid.NewGuid(),
                Title = "Test Snippet",
                Language = "c#",
                IsPrivate = false,
                Content = "Test Content",
                DateCreated = DateTime.UtcNow,
                DateExpires = DateTime.UtcNow.AddMinutes(60),
            };

            // Act
            using (var context = new PastebinContext(_contextOptions))
            {
                await context.AddAsync(testSnippet);
                await context.SaveChangesAsync();
            }

            // Assert
            using (var context = new PastebinContext(_contextOptions))
            {
                var repository = new SnippetRepository(context);
                var snippets = await repository.Get().Where(snippet => snippet.Id == testSnippet.Id).ToListAsync();

                Assert.Single(snippets);
                Assert.Equal(testSnippet.Id, snippets[0].Id);
                Assert.Equal(testSnippet.Title, snippets[0].Title);
                Assert.Equal(testSnippet.Language, snippets[0].Language);
                Assert.Equal(testSnippet.IsPrivate, snippets[0].IsPrivate);
                Assert.Equal(testSnippet.Content, snippets[0].Content);
                Assert.Equal(testSnippet.DateCreated, snippets[0].DateCreated);
                Assert.Equal(testSnippet.DateExpires, snippets[0].DateExpires);
            }
        }
    }
}