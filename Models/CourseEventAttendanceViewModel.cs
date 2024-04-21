namespace CCD_Attendance.Models
{
    public class CourseEventAttendanceViewModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public IEnumerable<EventAttendanceDetail> Events { get; set; }
    }

    public class EventAttendanceDetail
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public bool HasAttendance { get; set; }
    }
}
