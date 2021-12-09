using FeedbackSystem.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace FeedbackSystem.Api.Dtos
{
    public class FeedbackUpdateDto
    {
        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string EmailId { get; set; }

        [Required]
        public Feedback Feedback { get; set; }
    }
}
