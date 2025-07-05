using System.ComponentModel.DataAnnotations;

namespace CompetitionsWebsite.ViewModels
{
    public class RamadanQuestionInputViewModel
    {
        public int QuestionNumber { get; set; }

        [Required(ErrorMessage = "يرجى إدخال نص السؤال")]
        public string QuestionText { get; set; }

        // فقط للسؤال الأول
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }
    }
}
