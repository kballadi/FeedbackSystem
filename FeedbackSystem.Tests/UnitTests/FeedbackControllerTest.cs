using AutoMapper;
using FeedbackSystem.Api.Controllers;
using FeedbackSystem.Api.Dtos;
using FeedbackSystem.Core.Entities;
using FeedbackSystem.Infrastructure.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace FeedbackSystem.Tests.UnitTests
{
    public class FeedbackControllerTest
    {
        private readonly Mock<IFeedbackRepo> repoStub;
        private readonly Mock<IMapper> mapperStub;
        private readonly Mock<ILogger<FeedbackController>> loggerStub;
        private readonly InMemoryRepo memoryRepo = new();

        public FeedbackControllerTest()
        {
            repoStub = new Mock<IFeedbackRepo>();
            mapperStub = new Mock<IMapper>();
            loggerStub = new Mock<ILogger<FeedbackController>>();
        }

        [Fact]
        public void GetFeedbackByEmailId_WithUnregisteredEmailId_NotFound()
        {
            //Arrange
            repoStub.Setup(repo => repo.GetFeedbackByEmailId(""))
                .Returns((UserFeedback)null);
            var controller = new FeedbackController(repoStub.Object, mapperStub.Object, loggerStub.Object);

            //Act
            var result = controller.GetFeedbackByEmailId(string.Empty);

            //Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void GetFeedbackByEmailId_WithExistingEmailId_ReturnsExpectedItem()
        {
            //Arrange
            var expectedItem = memoryRepo.GetFeedbackByEmailId("user1@example.com");
            repoStub.Setup(repo => repo.GetFeedbackByEmailId("user1@example.com"))
                .Returns(expectedItem);
            var controller = new FeedbackController(repoStub.Object, mapperStub.Object, loggerStub.Object);

            //Act
            var result = controller.GetFeedbackByEmailId("user1@example.com");
            var okOjectResult = new OkObjectResult(mapperStub.Object.Map<FeedbackReadDto>(expectedItem));

            //Assert
            result.Should().BeEquivalentTo(okOjectResult);

            //Assert.IsType<OkObjectResult>(result.Result);
            //var dto = result.Value;
            //Assert.Equal(expectedItem.UserId, dto.UserId);
            //Assert.Equal(expectedItem.EmailId, dto.EmailId);
            //Assert.Equal(expectedItem.Feedback, dto.Feedback);
        }

        [Fact]
        public void GetFeedbacks_WithExisitingItems_ReturnsAllFeedbacks()
        {
            //Arrange
            var expectedItems = memoryRepo.GetFeedbacks();
            repoStub.Setup(repo => repo.GetFeedbacks())
                .Returns(expectedItems);
            var controller = new FeedbackController(repoStub.Object, mapperStub.Object, loggerStub.Object);

            //Act
            var result = controller.GetFeedbacks();

            //Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }
    }
}
