namespace CCD_Attendance.Models
{
    public class EventDetailViewModel
    {
        public Event Event { get; set; }
        public List<StudentViewModel> Attendees { get; set; }
    }

    public class StudentViewModel
    {
        public int StudentId { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
