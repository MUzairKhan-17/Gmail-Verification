using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CRUD_Operation.Models
{
    [Index(nameof(u_email), IsUnique = true)]
    public class User
    {
        [Key]
        public int u_id { get; set; }

        [Required]
        public string u_name { get; set; }

        [Required]
        public string u_email { get; set; }

        [Required]
        public string u_password { get; set; }

        public string EmailOTP { get; set; }

        public bool IsEmailVerified { get; set; } = false;
    }

}
