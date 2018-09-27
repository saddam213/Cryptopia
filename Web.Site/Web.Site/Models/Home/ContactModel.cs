using System.ComponentModel.DataAnnotations;


namespace Web.Site.Models
{

	public class ContactModel
    {
        [EmailAddress]
        [Display(Name = nameof(Resources.Home.contactEmailLabel), ResourceType = typeof(Resources.Home))]
        [Required(ErrorMessageResourceName = nameof(Resources.Home.contactEmailRequiredError), ErrorMessageResourceType = typeof(Resources.Home))]
        public string Email { get; set; }

        [Display(Name = nameof(Resources.Home.contactSubjectLabel), ResourceType = typeof(Resources.Home))]
        [Required(ErrorMessageResourceName = nameof(Resources.Home.contactSubjectRequiredError), ErrorMessageResourceType = typeof(Resources.Home))]
        public string Subject { get; set; }

        [Display(Name = nameof(Resources.Home.contactMessageLabel), ResourceType = typeof(Resources.Home))]
        [Required(ErrorMessageResourceName = nameof(Resources.Home.contactMessageRequiredError), ErrorMessageResourceType = typeof(Resources.Home))]
        public string Message { get; set; }
    }
}