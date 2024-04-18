using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CCD_Attendance.Models
{
    public class Event
    {

        public int EventID { get; set; }

        [Required]
        public string EventName { get; set; }

        [Required]
        public DateOnly EventDate { get; set; }

        public string EventDetails { get; set; }

    }
}
