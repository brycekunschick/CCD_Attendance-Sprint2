namespace CCD_Attendance.Areas.Admin.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
    }

    public class UsersViewModel
    {
        public List<UserViewModel> RequestedUsers { get; set; }
        public List<UserViewModel> RegularUsers { get; set; }
    }
}