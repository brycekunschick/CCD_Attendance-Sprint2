using System.Collections.Generic;

namespace CCD_Attendance.Models
{
    public class AttendanceDetailViewModel
    {
        public Course Course { get; set; }
        public Event Event { get; set; }
        public List<Student> Students { get; set; }
    }
}
