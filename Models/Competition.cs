namespace CompetitionsWebsite.Models
{
    public class Competition
    {
        public int Id { get; set; }
        public string Ttile { get; set; }
        public string Description { get; set; }


        public ICollection<Question> Questions { get; set; }

        public int CategoryId { get; set; }

        // Navigation property for the category
        public Category Category { get; set; }

        // Additional properties can be added as needed
        // For example, prize details, registration link, etc.
    }
}
