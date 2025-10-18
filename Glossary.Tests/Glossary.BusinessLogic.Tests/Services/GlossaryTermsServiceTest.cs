using FluentAssertions;
using Glossary.BusinessLogic.Configurations;
using Glossary.BusinessLogic.Exceptions;
using Glossary.BusinessLogic.Services;
using Glossary.DataAccess.Entities;
using Glossary.DataAccess.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using Moq;

namespace Glossary.Tests.Glossary.BusinessLogic.Tests.Services
{
    public class GlossaryTermsServiceTest
    {
        private readonly Mock<IGlossaryTermsRepository> _glossaryRepoMock;
        private readonly GlossaryTermsService _service;
        public GlossaryTermsServiceTest()
        {
            _glossaryRepoMock = new Mock<IGlossaryTermsRepository>();
            var forbiddenWordsRepoMock = new Mock<IForbiddenWordsRepository>();
            var optionsMock = Options.Create(new GlossarySettings { MinDefinitionLength = 30 });
            _service = new GlossaryTermsService(_glossaryRepoMock.Object, optionsMock, forbiddenWordsRepoMock.Object);
        }

        [Fact]
        public async Task GetById_WhenTermExists_ShouldReturnGlossaryTerm()
        {
            // Arrange
            var term = new GlossaryTerm { Id = 1, Term = "Test", Definition = "Test definition" };
            _glossaryRepoMock.Setup(r => r.GetById(1)).ReturnsAsync(term);

            // Act
            var result = await _service.GetById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(term, result);
        }

        [Fact]
        public async Task GetById_WhenTermDoesNotExist_ShouldThrowNotFoundException()
        {
            // Arrange
            _glossaryRepoMock.Setup(r => r.GetById(999)).ReturnsAsync((GlossaryTerm)null);

            // Act - Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetById(999));
        }

        [Fact]
        public async Task Delete_WhenInvalidStatus_ShouldThrowConflictException()
        {
            // Arrange
            var term = new GlossaryTerm { Id = 1, AuthorId = "user1", Status = Status.Published };
            _glossaryRepoMock.Setup(r => r.GetById(1)).ReturnsAsync(term);

            // Act - Assert
            await Assert.ThrowsAsync<ConflictException>(() => _service.Delete(1, "user1"));
        }

        [Fact]
        public async Task Delete_WhenInvalidUser_ShouldThrowForbidException()
        {
            // Arrange
            var term = new GlossaryTerm { Id = 1, AuthorId = "user1", Status = Status.Draft };
            _glossaryRepoMock.Setup(r => r.GetById(1)).ReturnsAsync(term);

            // Act & Assert
            await Assert.ThrowsAsync<ForbidException>(() => _service.Delete(1, "user2"));

        }

        [Fact]
        public async Task Delete_WhenValidUserAndStatus_ShouldCallRepositoryDelete()
        {
            // Arrange
            var term = new GlossaryTerm { Id = 1, AuthorId = "user1", Status = Status.Draft };
            _glossaryRepoMock.Setup(r => r.GetById(1)).ReturnsAsync(term);

            // Act
            await _service.Delete(1, "user1");

            // Assert
            _glossaryRepoMock.Verify(r => r.Delete(term), Times.Once);
        }

        [Fact]
        public async Task Archive_WhenInvalidStatus_ShouldThrowConflictException()
        {
            // Arrange
            var term = new GlossaryTerm { Id = 1, AuthorId = "user1", Status = Status.Draft };
            _glossaryRepoMock.Setup(r => r.GetById(1)).ReturnsAsync(term);

            // Act
            var act = async () => await _service.Archive(1, "user1");

            // Assert
            await act.Should().ThrowAsync<ConflictException>();
            _glossaryRepoMock.Verify(r => r.Update(It.IsAny<GlossaryTerm>()), Times.Never);
        }

        [Fact]
        public async Task Archive_WhenInvalidUser_ShouldThrowForbidException()
        {
            // Arrange
            var term = new GlossaryTerm { Id = 1, AuthorId = "user1", Status = Status.Published };
            _glossaryRepoMock.Setup(r => r.GetById(1)).ReturnsAsync(term);

            // Act
            var act = async () => await _service.Archive(1, "user2");

            // Assert
            await act.Should().ThrowAsync<ForbidException>();
            _glossaryRepoMock.Verify(r => r.Update(It.IsAny<GlossaryTerm>()), Times.Never);
        }

        [Fact]
        public async Task Archive_WhenValidUserAndStatus_ShouldSetStatusToArchived()
        {
            // Arrange
            var term = new GlossaryTerm { Id = 1, AuthorId = "user1", Status = Status.Published };
            _glossaryRepoMock.Setup(r => r.GetById(1)).ReturnsAsync(term);

            // Act
            await _service.Archive(1, "user1");

            // Assert
            term.Status.Should().Be(Status.Archived);
            _glossaryRepoMock.Verify(r => r.Update(term), Times.Once);
        }
    }
}
