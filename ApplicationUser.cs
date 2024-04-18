using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CCD_Attendance
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string AccessType { get; set; }



    }
}
