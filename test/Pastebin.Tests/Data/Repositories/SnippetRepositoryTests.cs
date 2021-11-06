using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
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
        public async Task AddAsync_ShouldAddSnippet_WhenSnippetIsValid()
        {
            // Arrange
            var testDateCreated = DateTime.UtcNow;
            var testSnippet = new Snippet
            {
                Id = Guid.NewGuid(),
                Title = "Test Snippet",
                Language = "c#",
                IsPrivate = false,
                Content = "Test Content",
                DateCreated = testDateCreated,
                DateExpires = testDateCreated.AddMinutes(60),
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

                snippet.Should().BeEquivalentTo(testSnippet);
            }
        }

        [Fact]
        public async Task FindAsync_ShouldReturnSnippet_WhenSnippetExists()
        {
            // Arrange
            var testDateCreated = DateTime.UtcNow;
            var testSnippet = new Snippet
            {
                Id = Guid.NewGuid(),
                Title = "Test Snippet",
                Language = "c#",
                IsPrivate = false,
                Content = "Test Content",
                DateCreated = testDateCreated,
                DateExpires = testDateCreated.AddMinutes(60),
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

                snippet.Should().BeEquivalentTo(testSnippet);
            }
        }


        [Fact]
        public async Task FindAsync_ShouldReturnNull_WhenSnippetDoesNotExist()
        {
            // Arrange & Act
            var testId = Guid.NewGuid();

            // Assert
            using (var context = new PastebinContext(_contextOptions))
            {
                var repository = new SnippetRepository(context);
                var snippet = await repository.FindAsync(testId);

                snippet.Should().BeNull();
            }
        }

        [Fact]
        public async Task Get_ShouldReturnSnippets_WhenQueried()
        {
            // Arrange
            var testDateCreated = DateTime.UtcNow;
            var testSnippet = new Snippet
            {
                Id = Guid.NewGuid(),
                Title = "Test Snippet",
                Language = "c#",
                IsPrivate = false,
                Content = "Test Content",
                DateCreated = testDateCreated,
                DateExpires = testDateCreated.AddMinutes(60),
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

                snippets.Should().ContainSingle().Which.Should().BeEquivalentTo(testSnippet);
            }
        }
    }
}