using AutoMapper;
using FeedbackSystem.Api.Controllers;
using FeedbackSystem.Api.Dtos;
using FeedbackSystem.Api.Profiles;
using FeedbackSystem.Core.Entities;
using FeedbackSystem.Infrastructure.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace FeedbackSystem.Tests.UnitTests
{
    public class FeedbackControllerTest
    {
        private readonly Mock<IFeedbackRepo> repoStub;
        private readonly Mock<IMapper> mapperStub;
        private readonly Mock<ILogger<FeedbackController>> loggerStub;
        private readonly Mock<IObjectModelValidator> validatorStub = new();
        private readonly InMemoryRepo memoryRepo = new();
        private readonly IMapper mapper;

        public FeedbackControllerTest()
        {
            repoStub = new Mock<IFeedbackRepo>();
            mapperStub = new Mock<IMapper>();
            loggerStub = new Mock<ILogger<FeedbackController>>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapperProfile());
            });
            mapper = mappingConfig.CreateMapper();

            validatorStub.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                                             It.IsAny<ValidationStateDictionary>(),
                                             It.IsAny<string>(),
                                             It.IsAny<Object>()));
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
            result.Result.Should().BeOfType<NotFoundResult>();
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

            //Assert
            result.Should().BeEquivalentTo(mapper.Map<FeedbackReadDto>(expectedItem), options => options.ExcludingMissingMembers());
        }

        [Fact]
        public void GetFeedbacks_WithExisitingItems_ReturnsAllFeedbacks()
        {
            //Arrange
            var expectedItems = memoryRepo.GetFeedbacks();
            repoStub.Setup(repo => repo.GetFeedbacks())
                .Returns(expectedItems);
            var controller = new FeedbackController(repoStub.Object, mapper, loggerStub.Object);

            //Act
            var result = controller.GetFeedbacks();
            var feedbacks = (result.Result as OkObjectResult).Value as IEnumerable<FeedbackReadDto>;

            //Assert
            feedbacks.Should().BeEquivalentTo(mapper.Map<IEnumerable<FeedbackReadDto>>(expectedItems), options => options.ComparingByMembers<FeedbackReadDto>());
        }

        [Fact]
        public void AddFeedback_WithValidItem_ReturnsAddedFeedback()
        {
            //Arrange            
            var controller = new FeedbackController(repoStub.Object, mapper, loggerStub.Object);
            controller.ObjectValidator = validatorStub.Object;

            //Act
            var createdFeedback = new FeedbackCreateDto() { EmailId = "newUserTest@email.com", Feedback = Feedback.Occasionally, Question = "Is your manager reachable" };
            var result = controller.AddFeedback(createdFeedback);

            //Assert
            var createdItem = (result.Result as CreatedAtRouteResult).Value as FeedbackReadDto;
            createdFeedback.Should().BeEquivalentTo(createdItem, options => options.ComparingByMembers<FeedbackReadDto>().ExcludingMissingMembers());
        }

        [Fact]
        public void UpdateFeedback_WithValidItems_ReturnsNoContent()
        {
            //Arrange
            var existingFeedback = memoryRepo.GetFeedbackByEmailId("user4@example.com");
            repoStub.Setup(repo => repo.GetFeedbackByEmailId("user4@example.com"))
                .Returns(existingFeedback);

            var updatedFeedback = new FeedbackUpdateDto() { EmailId = "updateEmail@email.com", Feedback = Feedback.Occasionally };

            var controller = new FeedbackController(repoStub.Object, mapperStub.Object, loggerStub.Object);

            //Act
            var result = controller.UpdateFeedback(existingFeedback.EmailId, updatedFeedback);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public void UpdateFeedback_WithNoEmailId_ReturnsNoFound()
        {
            //Arrange
            var updatedFeedback = new FeedbackUpdateDto() { EmailId = "updateEmail@email.com", Feedback = Feedback.Occasionally };

            var controller = new FeedbackController(repoStub.Object, mapperStub.Object, loggerStub.Object);

            //Act
            var result = controller.UpdateFeedback("noeamilid@exists.com", updatedFeedback);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void PartialUpdateFeedback_WithEmailId_ReturnsNoContent()
        {
            ////Arrange
            //var expectedItem = memoryRepo.GetFeedbackByEmailId("user1@example.com");
            //repoStub.Setup(repo => repo.GetFeedbackByEmailId("user1@example.com"))
            //    .Returns(expectedItem);
            //var jsonPatchDocument = new JsonPatchDocument<FeedbackUpdateDto>();

            //var updateEmailId = new FeedbackUpdateDto()
            //{
            //    EmailId = "partialUpdateTest@example.com"
            //};

            //jsonPatchDocument.Add(x => x, updateEmailId);

            //var controller = new FeedbackController(repoStub.Object, mapper, loggerStub.Object);
            //controller.ObjectValidator = validatorStub.Object;

            ////Act
            //var result = controller.PartialUpdateFeedback("user1@example.com", jsonPatchDocument);

            ////Assert
            //result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public void DeleteFeedback_WithExistingEmailId_ReturnsNoContent()
        {
            //Arrange
            var existingFeedback = memoryRepo.GetFeedbackByEmailId("user4@example.com");
            repoStub.Setup(repo => repo.GetFeedbackByEmailId("user4@example.com"))
                .Returns(existingFeedback);

            var controller = new FeedbackController(repoStub.Object, mapperStub.Object, loggerStub.Object);

            //Act
            var result = controller.DeleteFeedback(existingFeedback.EmailId);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public void DeleteFeedback_WithNoEmailId_ReturnsNoFound()
        {
            //Arrange
            var controller = new FeedbackController(repoStub.Object, mapperStub.Object, loggerStub.Object);
            //Act
            var result = controller.DeleteFeedback("NoEmailExisting@example.com");

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
