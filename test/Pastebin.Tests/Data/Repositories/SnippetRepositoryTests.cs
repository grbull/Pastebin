using System;
using System.Collections.Generic;
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

        private async Task ResetDatabase()
        {
            // Arrange
            using (var context = new PastebinContext(_contextOptions))
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
            }
        }

        [Fact]
        public async Task AddAsync_ShouldAddSnippet_WhenSnippetIsValid()
        {
            // Arrange
            await ResetDatabase();

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
            await ResetDatabase();

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
            // Arrange
            await ResetDatabase();

            var testId = Guid.NewGuid();

            // Act & Assert
            using (var context = new PastebinContext(_contextOptions))
            {
                var repository = new SnippetRepository(context);
                var snippet = await repository.FindAsync(testId);

                snippet.Should().BeNull();
            }
        }

        [Fact]
        public async Task GetRecentAsync_ShouldReturnAListOfSnippets()
        {
            // Arrange
            await ResetDatabase();

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
                var snippets = await repository.GetRecentAsync(1);

                snippets.Should().BeOfType<List<Snippet>>()
                    .And.ContainSingle().Which.Id.Should().Be(testSnippet.Id);
            }
        }

        [Fact]
        public async Task GetRecentAsync_ShouldNotReturnPrivateSnippets()
        {
            // Arrange
            await ResetDatabase();

            var testSnippet = new Snippet
            {
                Id = Guid.NewGuid(),
                IsPrivate = true,
                Content = "Test Content",
                DateCreated = DateTime.UtcNow,
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
                var snippets = await repository.GetRecentAsync(1);

                snippets.Should().BeOfType<List<Snippet>>().And.BeEmpty();
            }
        }

        [Fact]
        public async Task GetRecentAsync_ShouldNotReturnExpiredSnippets()
        {
            // Arrange
            await ResetDatabase();

            var testSnippet = new Snippet
            {
                Id = Guid.NewGuid(),
                IsPrivate = false,
                Content = "Test Content",
                DateCreated = DateTime.MinValue,
                DateExpires = DateTime.MinValue.AddMinutes(60)
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
                var snippets = await repository.GetRecentAsync(1);

                snippets.Should().BeOfType<List<Snippet>>().And.BeEmpty();
            }
        }

        [Fact]
        public async Task GetRecentAsync_ShouldReturnSnippetsDescendingByDateCreated()
        {
            // Arrange
            await ResetDatabase();

            var testSnippets = new List<Snippet>
            {
                new()
                {
                    Id = Guid.NewGuid(), Content = "Oldest", IsPrivate = false, DateCreated = DateTime.UtcNow
                },
                new()
                {
                    Id = Guid.NewGuid(), Content = "Test Content", IsPrivate = false,
                    DateCreated = DateTime.UtcNow.AddMinutes(30)
                },
                new()
                {
                    Id = Guid.NewGuid(), Content = "Newest", IsPrivate = false,
                    DateCreated = DateTime.UtcNow.AddMinutes(60)
                }
            };
            testSnippets.Sort((a, b) => DateTime.Compare(b.DateCreated, a.DateCreated));

            // Act
            using (var context = new PastebinContext(_contextOptions))
            {
                foreach (var testSnippet in testSnippets)
                {
                    await context.AddAsync(testSnippet);
                }

                await context.SaveChangesAsync();
            }

            // Assert
            using (var context = new PastebinContext(_contextOptions))
            {
                var repository = new SnippetRepository(context);
                var snippets = await repository.GetRecentAsync(3);

                snippets.Should().NotBeEmpty().And.HaveCount(3).And
                    .BeEquivalentTo(testSnippets, options => options.WithStrictOrdering());
            }
        }
    }
}