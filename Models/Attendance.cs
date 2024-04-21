using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CCD_Attendance.Models
{
    public class Attendance
    {

        public int AttendanceId { get; set; }

        [Required]
        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Student Student { get; set; }

        [Required]
        public int EventId { get; set; }
        [ForeignKey("EventId")]
        public Event Event { get; set; }

    }
}
