using FeedbackSystem.Core.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FeedbackSystem.Api.Dtos
{
    public class FeedbackReadDto
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string EmailId { get; set; }

        [Required]
        public Feedback Feedback { get; set; }

        [Required]
        [DefaultValue("Is your manager reachable")]
        public string Question
        {
            get; set;
        }

    }
}
