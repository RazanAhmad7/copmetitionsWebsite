using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CompetitionsWebsite.Models;

public class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Participant> Participants { get; set; }
    public DbSet<UserQuizAttempt> UserQuizAttempts { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<MCQQuestion> MCQQuestions { get; set; }
    public DbSet<SpellingQuestion> SpellingQuestions { get; set; }
    public DbSet<MatchingQuestion> MatchingQuestions { get; set; }
    public DbSet<UserAnswer> UserAnswers { get; set; }
    public DbSet<MCQOption> MCQOptions { get; set; }
    public DbSet<MatchingPair> MatchingPairs { get; set; }
    public DbSet<SpellingLetter> SpellingLetters { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<SpecialQuiz> SpecialQuizzes { get; set; }
    public DbSet<SpecialQuizAssignment> SpecialQuizAssignments { get; set; }
    public DbSet<SpecialQuizQuestion> SpecialQuizQuestions { get; set; }
    public DbSet<RamadanCompetitionQuestion> RamadanCompetitionQuestions { get; set; }
    public DbSet<RamadanUserAnswer> RamadanCompetitionAnswers { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Question>()
            .HasOne(q => q.Category)
            .WithMany(c => c.Questions)
            .HasForeignKey(q => q.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}