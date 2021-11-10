using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Pastebin.Web.Controllers;
using Pastebin.Web.Models;
using Pastebin.Web.Services;
using Xunit;

namespace Pastebin.Tests.Controllers
{
    public class HomeControllerTest
    {
        private readonly Mock<ILogger<SnippetController>> _logger = new Mock<ILogger<SnippetController>>();
        private readonly Mock<ISnippetService> _snippetService = new Mock<ISnippetService>();

        // https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/testing?view=aspnetcore-6.0
        // https://docs.microsoft.com/en-us/aspnet/web-api/overview/testing-and-debugging/unit-testing-controllers-in-web-api
        // https://stackoverflow.com/questions/8818207/how-should-one-unit-test-a-net-mvc-controller
        // https://stackoverflow.com/questions/43424095/how-to-unit-test-with-ilogger-in-asp-net-core

        [Fact]
        public void Create_GET_ShouldReturnCreateView()
        {
            // Arrange
            var snippetController = new SnippetController(_logger.Object, _snippetService.Object);

            // Act
            var result = snippetController.Create();

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().BeNull();
        }

        [Fact]
        public async Task Create_POST_ShouldReturnCreateView_WhenModelIsNotValid()
        {
            // Arrange
            var testSnippetCreateModel = new SnippetCreateModel
            {
                IsPrivate = false
            };
            var snippetController = new SnippetController(_logger.Object, _snippetService.Object);
            snippetController.ModelState.AddModelError("Content", "Required.");

            // Act
            var result = await snippetController.Create(testSnippetCreateModel);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().BeNull();
        }

        [Fact]
        public async Task Create_POST_ShouldRedirectToViewView_WhenModelIsValid()
        {
            // Arrange
            var testSnippetCreateModel = new SnippetCreateModel
            {
                Content = "Test Content",
                IsPrivate = false
            };
            var testSnippetModel = new SnippetModel
            {
                Id = Guid.NewGuid(),
                IsPrivate = testSnippetCreateModel.IsPrivate,
                Content = testSnippetCreateModel.Content,
                DateCreated = DateTime.UtcNow
            };
            var snippetController = new SnippetController(_logger.Object, _snippetService.Object);
            _snippetService.Setup(service => service.CreateAsync(testSnippetCreateModel))
                .ReturnsAsync(testSnippetModel);

            // Act
            var result = await snippetController.Create(testSnippetCreateModel);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().And.BeEquivalentTo(new
            {
                ControllerName = "Snippet",
                ActionName = "View",
            });
        }

        [Fact]
        public async Task Create_POST_ShouldCallSnippetServiceCreateAsyncOnce_WhenModelIsValid()
        {
            // Arrange
            var testSnippetCreateModel = new SnippetCreateModel
            {
                Content = "Test Content",
                IsPrivate = false
            };
            var testSnippetModel = new SnippetModel
            {
                Id = Guid.NewGuid(),
                IsPrivate = testSnippetCreateModel.IsPrivate,
                Content = testSnippetCreateModel.Content,
                DateCreated = DateTime.UtcNow
            };
            var snippetController = new SnippetController(_logger.Object, _snippetService.Object);
            _snippetService.Setup(service => service.CreateAsync(testSnippetCreateModel))
                .ReturnsAsync(testSnippetModel);

            // Act
            await snippetController.Create(testSnippetCreateModel);

            // Assert
            _snippetService.Verify(service => service.CreateAsync(testSnippetCreateModel), Times.Once);
        }

        [Fact]
        public async Task View_GET_ShouldReturnSnippetView_WhenSnippetExists()
        {
            // Arrange
            var testSnippetModel = new SnippetModel
            {
                Id = Guid.NewGuid(),
                IsPrivate = false,
                Content = "Test Content",
                DateCreated = DateTime.UtcNow
                
            };
            var snippetController = new SnippetController(_logger.Object, _snippetService.Object);
            _snippetService.Setup(service => service.GetByIdAsync(testSnippetModel.Id))
                .ReturnsAsync(testSnippetModel);

            // Act
            var result = await snippetController.View(testSnippetModel.Id);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().BeNull();
        }
        
        [Fact]
        public async Task View_GET_ShouldContainSnippetModel_WhenSnippetExists()
        {
            // Arrange
            var testSnippetModel = new SnippetModel
            {
                Id = Guid.NewGuid(),
                IsPrivate = false,
                Content = "Test Content",
                DateCreated = DateTime.UtcNow
                
            };
            var snippetController = new SnippetController(_logger.Object, _snippetService.Object);
            _snippetService.Setup(service => service.GetByIdAsync(testSnippetModel.Id))
                .ReturnsAsync(testSnippetModel);

            // Act
            var result = await snippetController.View(testSnippetModel.Id);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.Model.Should().BeEquivalentTo(testSnippetModel);
        }
        

        [Fact]
        public async Task View_GET_ShouldCallSnippetServiceGetByIdAsyncOnce()
        {
            // Arrange
            var testSnippetModel = new SnippetModel
            {
                Id = Guid.NewGuid(),
                IsPrivate = false,
                Content = "Test Content",
                DateCreated = DateTime.UtcNow
                
            };
            var snippetController = new SnippetController(_logger.Object, _snippetService.Object);
            _snippetService.Setup(service => service.GetByIdAsync(testSnippetModel.Id))
                .ReturnsAsync(testSnippetModel);

            // Act
            await snippetController.View(testSnippetModel.Id);

            // Assert
            _snippetService.Verify(service => service.GetByIdAsync(testSnippetModel.Id), Times.Once);
        }

        [Fact]
        public async Task View_GET_ShouldReturnNotFoundResult_WhenSnippetDoesNotExist()
        {
            // Arrange
            var testGuid = Guid.NewGuid();
            var snippetController = new SnippetController(_logger.Object, _snippetService.Object);
            _snippetService.Setup(service => service.GetByIdAsync(testGuid))
                .ReturnsAsync(null as SnippetModel);

            // Act
            var result = await snippetController.View(testGuid);

            // Assert
            result.Should().BeOfType<NotFoundResult>().Which.StatusCode.Should().Be(404);
        }
    }
}