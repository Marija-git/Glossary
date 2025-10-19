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
        private readonly Mock<IGlossaryTermsRepository> _glossaryTermRepositoryMock;
        private readonly GlossaryTermsService _service;
        private readonly Mock<IForbiddenWordsRepository> _forbiddenWordsRepositoryMock;
        private readonly Mock<IOptions<GlossarySettings>> _optionsMock;
        public GlossaryTermsServiceTest()
        {
            _glossaryTermRepositoryMock = new Mock<IGlossaryTermsRepository>();
            _forbiddenWordsRepositoryMock = new Mock<IForbiddenWordsRepository>();

            _optionsMock = new Mock<IOptions<GlossarySettings>>();
            _optionsMock.Setup(o => o.Value).Returns(new GlossarySettings { MinDefinitionLength = 30 });

            _service = new GlossaryTermsService(_glossaryTermRepositoryMock.Object, _optionsMock.Object, _forbiddenWordsRepositoryMock.Object);
        }

        [Fact]
        public async Task GetById_WhenTermExists_ShouldReturnGlossaryTerm()
        {
            // Arrange
            var term = new GlossaryTerm { Id = 1, Term = "Test", Definition = "Test definition" };
            _glossaryTermRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(term);

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
            _glossaryTermRepositoryMock.Setup(r => r.GetById(999)).ReturnsAsync((GlossaryTerm)null);

            // Act - Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetById(999));
        }

        [Fact]
        public async Task Delete_WhenInvalidStatus_ShouldThrowConflictException()
        {
            // Arrange
            var term = new GlossaryTerm { Id = 1, AuthorId = "user1", Status = Status.Published };
            _glossaryTermRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(term);

            // Act - Assert
            await Assert.ThrowsAsync<ConflictException>(() => _service.Delete(1, "user1"));
        }

        [Fact]
        public async Task Delete_WhenInvalidUser_ShouldThrowForbidException()
        {
            // Arrange
            var term = new GlossaryTerm { Id = 1, AuthorId = "user1", Status = Status.Draft };
            _glossaryTermRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(term);

            // Act & Assert
            await Assert.ThrowsAsync<ForbidException>(() => _service.Delete(1, "user2"));

        }

        [Fact]
        public async Task Delete_WhenValidUserAndStatus_ShouldCallRepositoryDelete()
        {
            // Arrange
            var term = new GlossaryTerm { Id = 1, AuthorId = "user1", Status = Status.Draft };
            _glossaryTermRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(term);

            // Act
            await _service.Delete(1, "user1");

            // Assert
            _glossaryTermRepositoryMock.Verify(r => r.Delete(term), Times.Once);
        }

        [Fact]
        public async Task Archive_WhenInvalidStatus_ShouldThrowConflictException()
        {
            // Arrange
            var term = new GlossaryTerm { Id = 1, AuthorId = "user1", Status = Status.Draft };
            _glossaryTermRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(term);

            // Act
            var act = async () => await _service.Archive(1, "user1");

            // Assert
            await act.Should().ThrowAsync<ConflictException>();
            _glossaryTermRepositoryMock.Verify(r => r.Update(It.IsAny<GlossaryTerm>()), Times.Never);
        }

        [Fact]
        public async Task Archive_WhenInvalidUser_ShouldThrowForbidException()
        {
            // Arrange
            var term = new GlossaryTerm { Id = 1, AuthorId = "user1", Status = Status.Published };
            _glossaryTermRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(term);

            // Act
            var act = async () => await _service.Archive(1, "user2");

            // Assert
            await act.Should().ThrowAsync<ForbidException>();
            _glossaryTermRepositoryMock.Verify(r => r.Update(It.IsAny<GlossaryTerm>()), Times.Never);
        }

        [Fact]
        public async Task Archive_WhenValidUserAndStatus_ShouldSetStatusToArchived()
        {
            // Arrange
            var term = new GlossaryTerm { Id = 1, AuthorId = "user1", Status = Status.Published };
            _glossaryTermRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(term);

            // Act
            await _service.Archive(1, "user1");

            // Assert
            term.Status.Should().Be(Status.Archived);
            _glossaryTermRepositoryMock.Verify(r => r.Update(term), Times.Once);
        }

        [Fact]
        public async Task Publish_WhenDefinitionContainsForbiddenWord_ShouldThrowBadRequest()
        {
            // arrange
            var term = new GlossaryTerm { Id = 1, AuthorId = "user1", Term = "Term", Definition = "This definition contains forbiddenWord." };
            var forbidden = new List<ForbiddenWord> { new ForbiddenWord { Word = "forbiddenWord" } };
            _glossaryTermRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(term);
            _forbiddenWordsRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(forbidden);
            var service = new GlossaryTermsService(_glossaryTermRepositoryMock.Object, _optionsMock.Object, _forbiddenWordsRepositoryMock.Object);

            // act
            var act = async () => await service.Publish(1, term, "user1");

            // assert
            await act.Should().ThrowAsync<BadRequestException>();
        }

        [Fact]
        public async Task Publish_WhenDefinitionTooShort_ShouldThrowBadRequest()
        {
            // arrange
            _optionsMock.Setup(o => o.Value).Returns(new GlossarySettings { MinDefinitionLength = 100 });
            var term = new GlossaryTerm { Id = 1, AuthorId = "user1", Term = "Term", Definition = "This definition is short." };
            _glossaryTermRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(term);
            var service = new GlossaryTermsService(_glossaryTermRepositoryMock.Object, _optionsMock.Object, _forbiddenWordsRepositoryMock.Object);

            // act
            var act = async () => await service.Publish(1, term, "user1");

            await act.Should().ThrowAsync<BadRequestException>();
        }

        [Fact]
        public async Task Publish__WhenTermIsEmpty_ShouldThrowBadRequest()
        {
            // arrange
            var term = new GlossaryTerm { Id = 1, AuthorId = "user1", Term = "   ", Definition = "Valid definition" };
            _glossaryTermRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(term);
            var service = new GlossaryTermsService(_glossaryTermRepositoryMock.Object, _optionsMock.Object, _forbiddenWordsRepositoryMock.Object);

            // act
            var act = async () => await service.Publish(1, term, "user1");

            // assert
            await act.Should().ThrowAsync<BadRequestException>();
        }

        [Fact]
        public async Task Publish_WhenInvalidUser_ShouldThrowForbidException()
        {
            var term = new GlossaryTerm { Id = 1, AuthorId = "user1", Term = "Term", Definition = "Valid definition" };
            _glossaryTermRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(term);
            var service = new GlossaryTermsService(_glossaryTermRepositoryMock.Object, _optionsMock.Object, _forbiddenWordsRepositoryMock.Object);

            var act = async () => await service.Publish(1, term, "user2");

            await act.Should().ThrowAsync<ForbidException>();
            _glossaryTermRepositoryMock.Verify(r => r.Update(It.IsAny<GlossaryTerm>()), Times.Never);
        }

        [Fact]
        public async Task Publish_WhenValidInput_ShouldSetStatusToPublished()
        {
            // arrange
            _optionsMock.Setup(o => o.Value).Returns(new GlossarySettings { MinDefinitionLength = 5 });
            var term = new GlossaryTerm { Id = 1, AuthorId = "user1", Term = "Term", Definition = "Valid definition" };
            _glossaryTermRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(term);
            _forbiddenWordsRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ForbiddenWord>());
            var service = new GlossaryTermsService(_glossaryTermRepositoryMock.Object, _optionsMock.Object, _forbiddenWordsRepositoryMock.Object);

            // act
            await service.Publish(1, new GlossaryTerm { Term = "  Term  ", Definition = "  Valid definition  " }, "user1");

            // assert
            term.Status.Should().Be(Status.Published);
            term.Term.Should().Be("Term");
            term.Definition.Should().Be("Valid definition");
            _glossaryTermRepositoryMock.Verify(r => r.Update(term), Times.Once);
        }

        [Fact]
        public async Task Create_WhenTermAlreadyExists_ShouldThrowConflictException()
        {
            // arrange
            var existing = new GlossaryTerm { Id = 1, Term = "Term" };
            _glossaryTermRepositoryMock.Setup(r => r.GetByTerm("Term")).ReturnsAsync(existing);
            var term = new GlossaryTerm { Term = "Term", Definition = "Definition" };
            var service = new GlossaryTermsService(_glossaryTermRepositoryMock.Object, _optionsMock.Object, _forbiddenWordsRepositoryMock.Object);

            // act
            var act = async () => await service.Create(term, "user1");

            // assert
            await act.Should().ThrowAsync<ConflictException>();
            _glossaryTermRepositoryMock.Verify(r => r.Create(It.IsAny<GlossaryTerm>()), Times.Never);
        }
        [Fact]
        public async Task Create_WhenTermDoesNotExist_ShouldCreateNewTerm()
        {
            // arrange
            var term = new GlossaryTerm { Term = "Term", Definition = "Definition" };
            _glossaryTermRepositoryMock.Setup(r => r.GetByTerm("Term")).ReturnsAsync((GlossaryTerm)null);

            var service = new GlossaryTermsService(_glossaryTermRepositoryMock.Object, _optionsMock.Object, _forbiddenWordsRepositoryMock.Object);

            // act
            await service.Create(term, "user123");

            // assert
            term.AuthorId.Should().Be("user123");
            _glossaryTermRepositoryMock.Verify(r => r.Create(term), Times.Once);
        }
    }
}
