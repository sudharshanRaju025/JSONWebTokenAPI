using System.ComponentModel.DataAnnotations;

namespace JSONWebTokenAPI.Authentication
{
   
    public class Register
    {
        [Display(Name = "Username")]
        [Required(ErrorMessage = "User Name is Required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
