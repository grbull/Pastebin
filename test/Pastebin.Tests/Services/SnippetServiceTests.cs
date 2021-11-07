using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Pastebin.Web.Data.Entities;
using Pastebin.Web.Data.Repositories;
using Pastebin.Web.Models;
using Pastebin.Web.Services;
using Xunit;

namespace Pastebin.Tests.Services
{
    public class SnippetServiceTests
    {
        private readonly SnippetService _snippetService;
        private readonly Mock<ISnippetRepository> _snippetRepository = new Mock<ISnippetRepository>();

        public SnippetServiceTests()
        {
            _snippetService = new SnippetService(_snippetRepository.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldCallSnippetRepositoryAddAsyncOnce()
        {
            // Arrange
            var testSnippetCreateModel = new SnippetCreateModel()
            {
                Content = "Test Content",
                IsPrivate = false,
            };
            _snippetRepository.Setup(repo => repo.AddAsync(It.IsAny<Snippet>()))
                .ReturnsAsync<Snippet, ISnippetRepository, Snippet>(s => s);

            // Act
            var snippetModel = await _snippetService.CreateAsync(testSnippetCreateModel);


            // Assert
            _snippetRepository.Verify(repo => repo.AddAsync(It.IsAny<Snippet>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenCreateModelIsNull()
        {
            // Arrange
            var testSnippetCreateModel = null as SnippetCreateModel;

            // Act
            Func<Task> act = () => _snippetService.CreateAsync(testSnippetCreateModel);

            // Act & Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenContentIsNull()
        {
            // Arrange
            var testSnippetCreateModel = new SnippetCreateModel()
            {
                IsPrivate = false,
            };

            // Act
            Func<Task> act = () => _snippetService.CreateAsync(testSnippetCreateModel);

            // Act & Assert
            await act.Should().ThrowAsync<NullReferenceException>();
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenIsPrivateIsNull()
        {
            // Arrange
            var testSnippetCreateModel = new SnippetCreateModel()
            {
                Content = "Test Content",
            };

            // Act
            Func<Task> act = () => _snippetService.CreateAsync(testSnippetCreateModel);

            // Act & Assert
            await act.Should().ThrowAsync<NullReferenceException>();
        }

        [Fact]
        public async Task CreateAsync_ShouldGenerateGuid()
        {
            // Arrange
            var testSnippetCreateModel = new SnippetCreateModel()
            {
                Content = "Test Content",
                IsPrivate = false,
            };
            _snippetRepository.Setup(repo => repo.AddAsync(It.IsAny<Snippet>()))
                .ReturnsAsync<Snippet, ISnippetRepository, Snippet>(s => s);

            // Act
            var snippetModel = await _snippetService.CreateAsync(testSnippetCreateModel);


            // Assert
            snippetModel.Id.Should().NotBeEmpty();
        }

        [Fact]
        public async Task CreateAsync_ShouldAssignTitle_WhenTitleIsDefined()
        {
            // Arrange
            var testSnippetCreateModel = new SnippetCreateModel()
            {
                Title = "Test title",
                Content = "Test Content",
                IsPrivate = false,
            };
            _snippetRepository.Setup(repo => repo.AddAsync(It.IsAny<Snippet>()))
                .ReturnsAsync<Snippet, ISnippetRepository, Snippet>(s => s);

            // Act
            var snippetModel = await _snippetService.CreateAsync(testSnippetCreateModel);

            // Assert
            snippetModel.Title.Should().NotBeEmpty().And.Be(testSnippetCreateModel.Title);
        }

        [Fact]
        public async Task CreateAsync_ShouldAssignLanguage_WhenLanguageIsDefined()
        {
            // Arrange
            var testSnippetCreateModel = new SnippetCreateModel()
            {
                Language = "c#",
                Content = "Test Content",
                IsPrivate = false,
            };
            _snippetRepository.Setup(repo => repo.AddAsync(It.IsAny<Snippet>()))
                .ReturnsAsync<Snippet, ISnippetRepository, Snippet>(s => s);

            // Act
            var snippetModel = await _snippetService.CreateAsync(testSnippetCreateModel);

            // Assert
            snippetModel.Language.Should().NotBeEmpty().And.Be(testSnippetCreateModel.Language);
        }

        [Fact]
        public async Task CreateAsync_ShouldGenerateDateCreated()
        {
            // Arrange
            var testSnippetCreateModel = new SnippetCreateModel()
            {
                Content = "Test Content",
                IsPrivate = false,
            };
            _snippetRepository.Setup(repo => repo.AddAsync(It.IsAny<Snippet>()))
                .ReturnsAsync<Snippet, ISnippetRepository, Snippet>(s => s);

            // Act
            var snippetModel = await _snippetService.CreateAsync(testSnippetCreateModel);

            // Assert
            snippetModel.DateCreated.Should().BeBefore(DateTime.UtcNow); // Really just need to see it was defined.
        }

        [Fact]
        public async Task CreateAsync_ShouldAssignDateExpired_WhenExpiresInMinIsDefined()
        {
            // Arrange
            var testExpiresInMin = 60;
            var testSnippetCreateModel = new SnippetCreateModel()
            {
                Content = "Test Content",
                IsPrivate = false,
                ExpiresInMin = testExpiresInMin
            };
            _snippetRepository.Setup(repo => repo.AddAsync(It.IsAny<Snippet>()))
                .ReturnsAsync<Snippet, ISnippetRepository, Snippet>(s => s);

            // Act
            var snippetModel = await _snippetService.CreateAsync(testSnippetCreateModel);

            // Act & Assert
            snippetModel.DateExpires.Should().NotBeNull().And.Be(snippetModel.DateCreated.AddMinutes(testExpiresInMin));
        }

        [Fact]
        public async Task GetByIdAsync_ShouldCallSnippetRepositoryFindAsyncOnce()
        {
            // Arrange
            var testSnippet = new Snippet()
            {
                Id = Guid.NewGuid(),
                Content = "Test Content",
                IsPrivate = false,
                DateCreated = DateTime.UtcNow,
            };
            _snippetRepository.Setup(repo => repo.FindAsync(testSnippet.Id))
                .ReturnsAsync(testSnippet);

            // Act
            await _snippetService.GetByIdAsync(testSnippet.Id);


            // Assert
            _snippetRepository.Verify(repo => repo.FindAsync(testSnippet.Id), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldNotReturnSnippet_WhenItsExpired()
        {
            // Arrange
            var testDateCreated = DateTime.MinValue;
            var testSnippet = new Snippet()
            {
                Id = Guid.NewGuid(),
                Content = "Test Content",
                IsPrivate = false,
                DateCreated = testDateCreated,
                DateExpires = DateTime.MinValue.AddMinutes(60)
            };
            _snippetRepository.Setup(repo => repo.FindAsync(testSnippet.Id))
                .ReturnsAsync(testSnippet);

            // Act
            var snippet = await _snippetService.GetByIdAsync(testSnippet.Id);

            // Assert
            snippet.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnSnippetModel_WhenSnippetIsFound()
        {
            // Arrange
            var testSnippet = new Snippet()
            {
                Id = Guid.NewGuid(),
                Content = "Test Content",
                IsPrivate = false,
                DateCreated = DateTime.Now,
            };
            _snippetRepository.Setup(repo => repo.FindAsync(testSnippet.Id))
                .ReturnsAsync(testSnippet);

            // Act
            var snippet = await _snippetService.GetByIdAsync(testSnippet.Id);

            // Assert
            snippet.Should().BeEquivalentTo(testSnippet);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenSnippetIsNotFound()
        {
            // Arrange
            var testSnippetId = Guid.NewGuid();

            _snippetRepository.Setup(repo => repo.FindAsync(testSnippetId))
                .ReturnsAsync(null as Snippet);

            // Act
            var snippet = await _snippetService.GetByIdAsync(testSnippetId);

            // Assert
            snippet.Should().BeNull();
        }

        [Fact]
        public async Task GetRecentAsync_ShouldCallSnippetRepositoryGetOnce()
        {
            // Arrange
            var testCount = 10;
            var testSnippets = new List<Snippet>();
            _snippetRepository.Setup(repo => repo.GetRecentAsync(testCount))
                .ReturnsAsync(testSnippets);

            // Act
            await _snippetService.GetRecentAsync(testCount);

            // Assert
            _snippetRepository.Verify(repo => repo.GetRecentAsync(testCount), Times.Once);
        }

        [Fact]
        public async Task GetRecentAsync_ShouldReturnListOfSnippetModels()
        {
            // Arrange
            var testCount = 10;
            var testSnippets = new List<Snippet>
            {
                new() {Id = Guid.NewGuid(), Content = "Test content", IsPrivate = false, DateCreated = DateTime.UtcNow}
            };
            _snippetRepository.Setup(repo => repo.GetRecentAsync(testCount))
                .ReturnsAsync(testSnippets);

            // Act
            var snippetModels = await _snippetService.GetRecentAsync(testCount);

            // Assert
            snippetModels.Should().BeOfType<List<SnippetModel>>().And.ContainSingle().Which.Id.Should()
                .Be(testSnippets[0].Id);
        }
    }
}