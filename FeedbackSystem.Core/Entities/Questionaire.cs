using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FeedbackSystem.Core.Entities
{
    public class Questionaire
    {
        [Key]
        public int QuestionId { get; set; }

        [DefaultValue("Is your manager reachable")]
        public string Question
        {
            get; set;
        }

        [Required]
        [DefaultValue(Feedback.Always)]
        public Feedback Option1
        {
            get; set;
        }

        [Required]
        [DefaultValue(Feedback.Occasionally)]
        public Feedback Option2
        {
            get; set;
        }

        [Required]
        [DefaultValue(Feedback.TakesTime)]
        public Feedback Option3
        {
            get; set;
        }

        [Required]
        [DefaultValue(Feedback.NotReachable)]
        public Feedback Option4
        {
            get; set;
        }
    }
}
