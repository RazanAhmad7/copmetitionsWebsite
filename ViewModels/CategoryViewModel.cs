using System.ComponentModel.DataAnnotations;

namespace CompetitionsWebsite.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم القسم مطلوب")]
        public string Name { get; set; }

        [Required(ErrorMessage = "الوصف مطلوب")]
        public string Description { get; set; }

    }
}
