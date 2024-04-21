using System;

namespace CCD_Attendance.Models
{
    public class CourseViewModel
    {
        public int CourseId { get; set; }
        public int CRN { get; set; }
        public string CourseName { get; set; }
        public string CourseSection { get; set; }
        public string CourseSemester { get; set; }
        public int CourseYear { get; set; }
        public bool HasRoster { get; set; } // Indicates whether a roster has been uploaded for this course
    }
}
