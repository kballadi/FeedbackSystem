using FeedbackSystem.Core.Entities;
using FeedbackSystem.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FeedbackSystem.Tests
{
    /// <summary>
    /// This class is used for UnitTesting
    /// </summary>
    public class InMemoryRepo : IFeedbackRepo
    {
        private readonly List<UserFeedback> _feedbacks;

        public InMemoryRepo()
        {
            _feedbacks = new List<UserFeedback>
            {
                new UserFeedback{EmailId="user1@example.com",Feedback=Feedback.Always},
                new UserFeedback{EmailId="user2@example.com",Feedback=Feedback.Occasionally},
                new UserFeedback{EmailId="user3@example.com",Feedback=Feedback.NotReachable},
                new UserFeedback{EmailId="user4@example.com",Feedback=Feedback.TakesTime}
            };
        }

        public IEnumerable<UserFeedback> GetFeedbacks()
        {
            return _feedbacks;
        }

        public UserFeedback GetFeedbackByEmailId(string emailId)
        {
            return _feedbacks.FirstOrDefault(x => x.EmailId == emailId);
        }

        public void CreateFeedback(UserFeedback feedback)
        {
            if (feedback is null)
                throw new ArgumentNullException(nameof(feedback));
            _feedbacks.Add(feedback);
        }

        public void UpdateFeedback(UserFeedback feedback)
        {
            //Just return nothing
        }

        public void DeleteFeedback(string emailId)
        {
            var feedback = _feedbacks.FirstOrDefault(x => x.EmailId == emailId);
            if (feedback is null)
                throw new ArgumentNullException(nameof(feedback));
            _feedbacks.Remove(feedback);
        }

        public bool SaveChanges()
        {
            return true;
        }
    }
}
