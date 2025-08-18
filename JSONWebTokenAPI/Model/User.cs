using System.ComponentModel.DataAnnotations;

namespace JSONWebTokenAPI.Model
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage ="Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage ="Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage ="Phone number is required")]
        public string PhoneNo { get; set; }


        public string Address { get; set; }
    }
}
