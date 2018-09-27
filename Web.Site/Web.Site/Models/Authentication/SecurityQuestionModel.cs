using System.ComponentModel.DataAnnotations;

namespace Web.Site.Models
{
	public class SecurityQuestionModel
    {
        [Display(Name = "Question")]
        public string Question { get; set; }

        [Display(Name = "Answer")]
        public string Answer { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Question) && !string.IsNullOrEmpty(Answer);
        }
    }
}