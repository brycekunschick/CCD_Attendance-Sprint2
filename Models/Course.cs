using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CCD_Attendance.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        [Required]
        public int CRN { get; set; }

        [Required]
        public string CourseName { get; set; }

        [Required]
        public string CourseSection { get; set; }

        [Required]
        public string CourseSemester { get; set; }

        [Required]
        public int CourseYear { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }  // Navigational property
    }
}
