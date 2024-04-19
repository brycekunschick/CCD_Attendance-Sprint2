using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CCD_Attendance.Models
{
    public class Event
    {
        public int EventId { get; set; }

        [Required]
        public string EventName { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        public string EventDetails { get; set; }

        public string? EventNotesCCD { get; set; }

        public bool ApprovalStatus { get; set; } = false;

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }  // Navigational property
    }
}
