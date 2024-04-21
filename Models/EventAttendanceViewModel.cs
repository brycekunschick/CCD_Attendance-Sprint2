namespace CCD_Attendance.Models
{
    public class EventAttendanceViewModel
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public string EventDetails { get; set; }
        public string? EventNotesCCD { get; set; }
        public bool HasAttendance { get; set; }
    }
}
