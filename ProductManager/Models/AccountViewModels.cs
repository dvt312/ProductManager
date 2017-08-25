using System.ComponentModel.DataAnnotations;

namespace ProductManager.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Use Persistent Storage")]
        public bool UsePersistentStorage { get; set; }
    }
}
