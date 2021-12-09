using FeedbackSystem.Core.Entities;
using FeedbackSystem.Infrastructure.Data;
using FeedbackSystem.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace FeedbackSystem.Infrastructure.Repos
{
    public class FeedbackRepo : IFeedbackRepo
    {
        private readonly ILogger<FeedbackRepo> logger;
        private readonly FeedbackContext dbContext;
        public FeedbackRepo(ILogger<FeedbackRepo> logger, FeedbackContext context)
        {
            this.logger = logger;
            dbContext = context;
        }

        public void CreateFeedback(UserFeedback feedback)
        {
            logger.LogInformation("Adding New Feedback");
            var question = dbContext.Questions.FirstOrDefault();
            feedback.Questionaire = question;
            feedback.Question = question.Question;
            dbContext.Feedbacks.Add(feedback);
            SaveChanges();
        }

        public void DeleteFeedback(string emailId)
        {

            logger.LogInformation($"Deleting a Feedback with Email Id : {emailId}");
            var feedback = dbContext.Feedbacks.FirstOrDefault(x => x.EmailId == emailId);
            dbContext.Feedbacks.Remove(feedback);
            SaveChanges();

        }

        public UserFeedback GetFeedbackByEmailId(string emailId)
        {

            logger.LogInformation($"Getting a Feedback with Email Id : {emailId}");
            var feedback = dbContext.Feedbacks.Include(x => x.Questionaire).FirstOrDefault(x => x.EmailId == emailId);
            return feedback;
        }

        public IEnumerable<UserFeedback> GetFeedbacks()
        {

            logger.LogInformation($"Getting a Feedbacks");
            var question = dbContext.Questions.SingleOrDefault();
            var feedbacks = dbContext.Feedbacks.Include(x => x.Questionaire).ToList();
            return feedbacks;
        }

        public void UpdateFeedback(UserFeedback feedback)
        {

            logger.LogInformation($"Updating an existing Feedback");
            var existingFeedback = feedback;
            dbContext.Feedbacks.Update(existingFeedback);
            SaveChanges();
        }

        public bool SaveChanges()
        {
            return dbContext.SaveChanges() >= 0;
        }
    }
}
