using System.ComponentModel.DataAnnotations;


namespace Web.Site.Models
{


	public class SupportModel
    {
        [EmailAddress]
        [Display(Name = nameof(Resources.Support.supportEmailLabel), ResourceType = typeof(Resources.Support))]
        [Required(ErrorMessageResourceName = nameof(Resources.Support.supportEmailRequiredError), ErrorMessageResourceType = typeof(Resources.Support))]
        public string Email { get; set; }

        [Display(Name = nameof(Resources.Support.supportSubjectLabel), ResourceType = typeof(Resources.Support))]
        [Required(ErrorMessageResourceName = nameof(Resources.Support.supportSubjectRequiredError), ErrorMessageResourceType = typeof(Resources.Support))]
        public string Subject { get; set; }

        [Display(Name = nameof(Resources.Support.supportMessageLabel), ResourceType = typeof(Resources.Support))]
        [Required(ErrorMessageResourceName = nameof(Resources.Support.supportMessageRequiredError), ErrorMessageResourceType = typeof(Resources.Support))]
        public string Message { get; set; }

        public bool IsError { get; set; }
        public string Result { get; set; }
    }
}