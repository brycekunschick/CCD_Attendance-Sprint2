using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CCD_Attendance.Models
{
    public class UploadAttendanceViewModel
    {
        [Required(ErrorMessage = "Please upload a file.")]
        public IFormFile File { get; set; }
        public int EventId { get; set; }
    }
}
