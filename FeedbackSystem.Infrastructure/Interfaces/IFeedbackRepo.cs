using FeedbackSystem.Core.Entities;
using System.Collections.Generic;

namespace FeedbackSystem.Infrastructure.Interfaces
{
    public interface IFeedbackRepo
    {
        IEnumerable<UserFeedback> GetFeedbacks();
        UserFeedback GetFeedbackByEmailId(string emailId);
        void CreateFeedback(UserFeedback feedback);
        void UpdateFeedback(UserFeedback feedback);
        void DeleteFeedback(string emailId);
        bool SaveChanges();

    }
}
