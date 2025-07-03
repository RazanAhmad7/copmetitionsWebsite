using Microsoft.AspNetCore.Identity;

namespace CompetitionsWebsite.Models
{
    public class User : IdentityUser
    {
     public DateTime JoinedAt { get; set; }

    // You can also add navigation properties here
    public ICollection<UserQuizAttempt> UserQuizAttempts { get; set; }
    }
}
