using FeedbackSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace FeedbackSystem.Infrastructure.Data
{
    public class FeedbackContext : DbContext
    {
        public FeedbackContext(DbContextOptions<FeedbackContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserFeedback>()
                        .Property(u => u.Feedback)
                        .HasConversion<string>();

            modelBuilder.Entity<Questionaire>()
                        .Property(u => u.Option1)
                        .HasConversion<string>();
            modelBuilder.Entity<Questionaire>()
                        .Property(u => u.Option2)
                        .HasConversion<string>();
            modelBuilder.Entity<Questionaire>()
                        .Property(u => u.Option3)
                        .HasConversion<string>();
            modelBuilder.Entity<Questionaire>()
                        .Property(u => u.Option4)
                        .HasConversion<string>();
        }
        public DbSet<UserFeedback> Feedbacks { get; set; }
        public DbSet<Questionaire> Questions { get; set; }
    }
}
