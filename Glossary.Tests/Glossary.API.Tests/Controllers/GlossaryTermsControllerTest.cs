
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Glossary.API.Controllers;
using Glossary.API.DTOs.Request;
using Glossary.API.DTOs.Response;
using Glossary.BusinessLogic.Exceptions;
using Glossary.BusinessLogic.Services.Interfaces;
using Glossary.DataAccess.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;


namespace Glossary.Tests.Glossary.API.Tests.Controllers
{
    public class GlossaryTermsControllerTest
    {
        private readonly Mock<IGlossaryTermsService> _glossaryTermServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GlossaryTermsController _glossaryTermController;
        private readonly Fixture _fixture;

        public GlossaryTermsControllerTest()
        {
            _glossaryTermServiceMock = new Mock<IGlossaryTermsService>();
            _mapperMock = new Mock<IMapper>();
            _glossaryTermController = new GlossaryTermsController(_glossaryTermServiceMock.Object, _mapperMock.Object);
            _fixture = new Fixture();
            _fixture.Behaviors.Remove(_fixture.Behaviors.OfType<ThrowingRecursionBehavior>().FirstOrDefault());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        }

        [Fact]
        public async Task Create_ShouldReturnCreatedResult()
        {
            //arrange
            var userId = Guid.NewGuid().ToString();
            SimulateUser(userId);
            var glossaryTermDtoRequest = _fixture.Create<GlossaryTermDtoRequest>();
            var glossaryTermMapped = _fixture.Create<GlossaryTerm>();

            _mapperMock.Setup(m => m.Map<GlossaryTerm>(glossaryTermDtoRequest)).Returns(glossaryTermMapped);

            //act
            var result = await _glossaryTermController.Create(glossaryTermDtoRequest);

            //assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            createdResult.ActionName.Should().Be(nameof(_glossaryTermController.GetById));
            createdResult.RouteValues!["id"].Should().Be(glossaryTermMapped.Id);
            _glossaryTermServiceMock.Verify(s => s.Create(glossaryTermMapped, userId), Times.Once);
        }


        [Fact]
        public async Task GetById_WhenExists_ShouldReturnMappedDto()
        {
            //arrnage
            var glossaryTerm = _fixture.Create<GlossaryTerm>();
            var glossaryTermDto = _fixture.Create<GlossaryTermDtoResponse>();

            _glossaryTermServiceMock.Setup(s => s.GetById(glossaryTerm.Id)).ReturnsAsync(glossaryTerm);
            _mapperMock.Setup(m => m.Map<GlossaryTermDtoResponse>(glossaryTerm)).Returns(glossaryTermDto);

            //act
            var result = await _glossaryTermController.GetById(glossaryTerm.Id);

            //assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(glossaryTermDto);
            _glossaryTermServiceMock.Verify(s => s.GetById(glossaryTerm.Id), Times.Once);
        }

        [Fact]
        public async Task Delete_ShoulReturnNoContent()
        {
            //arrange
            var userId = Guid.NewGuid().ToString();
            SimulateUser(userId);
            var id = _fixture.Create<int>();

            _glossaryTermServiceMock.Setup(s => s.Delete(id, userId));

            //act
            var result = await _glossaryTermController.Delete(id);

            //assert
            result.Should().BeOfType<NoContentResult>();
            _glossaryTermServiceMock.Verify(s => s.Delete(id, userId), Times.Once);
        }

        [Fact]
        public async Task Archive_ShoulReturnNoContent()
        {
            //arrange
            var id = _fixture.Create<int>();
            var userId = Guid.NewGuid().ToString();
            SimulateUser(userId);

            _glossaryTermServiceMock.Setup(s => s.Archive(id, userId));

            //act
            var result = await _glossaryTermController.Archive(id);

            //assert
            result.Should().BeOfType<NoContentResult>();
            _glossaryTermServiceMock.Verify(s => s.Archive(id, userId), Times.Once);
        }

        [Fact]
        public async Task Publish_ShouldReturnNoContent()
        {
            //arrange
            var userId = Guid.NewGuid().ToString();
            SimulateUser(userId);

            var id = 1;
            var glossaryTermDtoRequest = _fixture.Create<GlossaryTermDtoRequest>();
            var glossaryTermMapped = _fixture.Create<GlossaryTerm>();

            _mapperMock.Setup(m => m.Map<GlossaryTerm>(glossaryTermDtoRequest)).Returns(glossaryTermMapped);
            _glossaryTermServiceMock.Setup(s => s.Publish(id, glossaryTermMapped, userId));

            //act
            var result = await _glossaryTermController.Publish(id, glossaryTermDtoRequest);

            //assert
            result.Should().BeOfType<NoContentResult>();
            _glossaryTermServiceMock.Verify(s => s.Publish(id, glossaryTermMapped, userId), Times.Once);

        }

        [Fact]
        public async Task GetPaged_ForUnauthenticatedUser_ShouldReturnPaginatedGlossaryTerms()
        {
            //arrange
            int pageSize = 10;
            int pageIndex = 1;
            string? userId = null;

            var paginatedGlossaryTerms = _fixture.Create<PaginatedData<GlossaryTerm>>();
            var paginatedGlossaryTermsMapped = _fixture.Create<PaginatedData<GlossaryTermDtoResponse>>();

            _glossaryTermServiceMock.Setup(s => s.GetGlossaryTermsPaged(userId, pageSize, pageIndex))
                .ReturnsAsync(paginatedGlossaryTerms);

            _mapperMock.Setup(m => m.Map<PaginatedData<GlossaryTermDtoResponse>>(paginatedGlossaryTerms))
                .Returns(paginatedGlossaryTermsMapped);

            _glossaryTermController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity())
                }
            };

            //act
            var result = await _glossaryTermController.GetPagedTerms(pageSize, pageIndex);

            //assert
            result.Should().BeEquivalentTo(paginatedGlossaryTermsMapped);
            _glossaryTermServiceMock.Verify(s => s.GetGlossaryTermsPaged(userId, pageSize, pageIndex), Times.Once);
        }

        [Fact]
        public async Task GetPaged_ForAuthenticatedUser_ShouldReturnPaginatedGlossaryTerms()
        {
            //arrange
            int pageSize = 10;
            int pageIndex = 1;
            var userId = Guid.NewGuid().ToString();
            SimulateUser(userId);

            var paginatedGlossaryTerms = _fixture.Create<PaginatedData<GlossaryTerm>>();
            var paginatedGlossaryTermsMapped = _fixture.Create<PaginatedData<GlossaryTermDtoResponse>>();

            _glossaryTermServiceMock.Setup(s => s.GetGlossaryTermsPaged(userId, pageSize, pageIndex))
                .ReturnsAsync(paginatedGlossaryTerms);

            _mapperMock.Setup(m => m.Map<PaginatedData<GlossaryTermDtoResponse>>(paginatedGlossaryTerms))
                .Returns(paginatedGlossaryTermsMapped);

            //act
            var result = await _glossaryTermController.GetPagedTerms(pageSize, pageIndex);

            //assert
            result.Should().BeEquivalentTo(paginatedGlossaryTermsMapped);
            _glossaryTermServiceMock.Verify(s => s.GetGlossaryTermsPaged(userId, pageSize, pageIndex), Times.Once);
        }

        private void SimulateUser(string userId)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));

            _glossaryTermController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }
    }
}
