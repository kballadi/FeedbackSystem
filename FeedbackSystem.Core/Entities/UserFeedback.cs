using System.ComponentModel.DataAnnotations;

namespace FeedbackSystem.Core.Entities
{
    public class UserFeedback
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string EmailId { get; set; }

        [Required]
        public Feedback Feedback
        {
            get; set;
        }

        [Required]
        public string Question
        {
            get; set;
        }

        //Navigation Properties
        // public int QuestionId
        // {
        //     get; set;
        // }

        public Questionaire Questionaire
        {
            get; set;
        }
    }

}
