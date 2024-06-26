﻿using CCD_Attendance.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace CCD_Attendance.Data
{
    public class ccdDBContext : IdentityDbContext<IdentityUser>
    {
        public ccdDBContext(DbContextOptions<ccdDBContext> options)
            : base(options)
        {

        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<Student> Students { get; set; }
        public DbSet<Attendance> Attendances { get; set; }

        public DbSet<CourseStudent> CourseStudents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            

        }



    }
}
