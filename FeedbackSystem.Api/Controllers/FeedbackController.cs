using AutoMapper;
using FeedbackSystem.Api.Dtos;
using FeedbackSystem.Core.Entities;
using FeedbackSystem.Core.Exceptions;
using FeedbackSystem.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace FeedbackSystem.Api.Controllers
{
    /// <summary>
    /// Feedback Controller Class
    /// </summary>
    [ApiController]
    [Route("api/feedbacks")]
    public class FeedbackController : ControllerBase
    {
        public readonly IFeedbackRepo feedbackRepo;
        private readonly IMapper _mapper;
        public readonly ILogger<FeedbackController> _logger;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="mapper"></param>
        /// <param name="logger"></param>
        public FeedbackController(IFeedbackRepo repo, IMapper mapper, ILogger<FeedbackController> logger)
        {
            feedbackRepo = repo;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Gets the feedbacks from the system.
        /// </summary>
        /// <returns>Feedbacks</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<UserFeedback>> GetFeedbacks()
        {
            _logger.LogInformation("Getting Feedbacks");
            var feedbacks = feedbackRepo.GetFeedbacks();

            if (feedbacks is null)
            {
                _logger.LogWarning("No feedbacks found");
                return NotFound();
            }
            return Ok(_mapper.Map<IEnumerable<FeedbackReadDto>>(feedbacks));
        }

        /// <summary>
        /// Gets the feedback from the system based on email id.
        /// </summary>
        /// <returns>Feedback</returns>
        [HttpGet("{emailId}", Name = "GetFeedbackByEmailId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<FeedbackReadDto> GetFeedbackByEmailId(string emailId)
        {
            _logger.LogInformation($"Getting feedback with {emailId}");
            var feedback = feedbackRepo.GetFeedbackByEmailId(emailId);
            if (feedback is null)
            {
                _logger.LogWarning($"Feedback with {emailId} not found.");
                return NotFound();
            }
            return Ok(_mapper.Map<FeedbackReadDto>(feedback));
        }

        /// <summary>
        /// Adds a feedback to the system.
        /// </summary>
        /// <returns>Feedback Created</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<FeedbackCreateDto> AddFeedback(FeedbackCreateDto feedbackCreateDto)
        {
            _logger.LogInformation($"Adding feedback in the system.");
            var feedbackModel = _mapper.Map<UserFeedback>(feedbackCreateDto);
            if (!TryValidateModel(feedbackModel))
            {
                _logger.LogWarning($"Adding feedback in the system failed {ModelState}.");
                return ValidationProblem(ModelState);
            }
            var emailId = feedbackRepo.GetFeedbackByEmailId(feedbackCreateDto.EmailId)?.EmailId;
            if (emailId != null && feedbackCreateDto.Question == feedbackModel.Question)
                throw new UserFeedbackSubmittedException($"User with emailId '{emailId}' already submitted feedback.");
            else if (Enum.TryParse(typeof(Feedback), feedbackCreateDto.Feedback.ToString(), out object feedback))
            {
                feedbackRepo.CreateFeedback(feedbackModel);
                var feedbackReadDto = _mapper.Map<FeedbackReadDto>(feedbackModel);
                return CreatedAtRoute(nameof(GetFeedbackByEmailId), new { EmailId = feedbackModel.EmailId }, feedbackReadDto);
            }
            return BadRequest();
        }

        /// <summary>
        /// Updates the feedback individual property based on email Id.
        /// </summary>
        /// <returns>Feedback</returns>
        [HttpPatch("{emailId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult PartialUpdateCommand(string emailId, JsonPatchDocument<FeedbackUpdateDto> patchDocument)
        {
            _logger.LogInformation($"Updating feedback with respective property {patchDocument}.");
            var existingFeedback = feedbackRepo.GetFeedbackByEmailId(emailId);
            var feedbackToPatch = _mapper.Map<FeedbackUpdateDto>(existingFeedback);
            patchDocument.ApplyTo(feedbackToPatch, ModelState);
            if (!TryValidateModel(feedbackToPatch))
            {
                _logger.LogWarning($"Updating feedback with respective property {ModelState} failed.");
                return ValidationProblem(ModelState);
            }
            if (Enum.TryParse(typeof(Feedback), feedbackToPatch.Feedback.ToString(), out object feedback))
            {
                _mapper.Map(feedbackToPatch, existingFeedback);
                feedbackRepo.UpdateFeedback(existingFeedback);
                return NoContent();
            }

            return BadRequest();
        }

        /// <summary>
        /// Updates the feedback in the system based email Id.
        /// </summary>
        /// <returns>Feedback</returns>
        [HttpPut("{emailId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult UpdateFeedback(string emailId, FeedbackUpdateDto feedbackUpdateDto)
        {
            _logger.LogInformation($"Updating feedback with emailId {emailId}.");
            var existingFeedback = feedbackRepo.GetFeedbackByEmailId(emailId);
            if (existingFeedback is null)
            {
                _logger.LogWarning($"Unable to update feedback with emailId {emailId}.");
                return NotFound();
            }
            if (Enum.TryParse(typeof(Feedback), feedbackUpdateDto.Feedback.ToString(), out object feedback))
            {
                _mapper.Map(feedbackUpdateDto, existingFeedback);
                feedbackRepo.UpdateFeedback(existingFeedback);
                return NoContent();
            }
            return BadRequest();
        }

        /// <summary>
        /// Delete a specific feedback in the system based on email id.
        /// </summary>
        /// <returns>Feedback</returns>
        [HttpDelete("{emailId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteFeedback(string emailId)
        {
            _logger.LogInformation($"Deleting feedback with emailId {emailId}.");
            var feedback = feedbackRepo.GetFeedbackByEmailId(emailId);
            if (feedback is null)
            {
                return NotFound();
            }
            feedbackRepo.DeleteFeedback(emailId);
            return NoContent();
        }
    }
}
