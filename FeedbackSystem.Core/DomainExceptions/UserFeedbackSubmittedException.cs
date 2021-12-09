using System;

namespace FeedbackSystem.Core.Exceptions
{
    public class UserFeedbackSubmittedException : Exception
    {
        public UserFeedbackSubmittedException(string message) : base(message)
        {

        }
    }
}
