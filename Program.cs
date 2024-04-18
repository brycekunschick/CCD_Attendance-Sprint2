using CCD_Attendance.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;



namespace CCD_Attendance
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();


            //1) fetch the information about the connection string
            var connString = builder.Configuration.GetConnectionString("DefaultConnection");

            //2) Add the context class to the set of services and define the option to use SQL Server on that connection string that has been fetched in the previous line
            builder.Services.AddDbContext<ccdDBContext>(options => options.UseSqlServer(connString));

            builder.Services.AddIdentity<IdentityUser,IdentityRole>().AddEntityFrameworkStores<ccdDBContext>().AddDefaultTokenProviders();



            builder.Services.AddRazorPages();

            builder.Services.AddScoped<IEmailSender, EmailSender>();



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapRazorPages();

            app.MapControllerRoute(
                name: "default",
                pattern: "{Area=Requested}/{controller=Home}/{action=UnauthorizedIndex}/{id?}");

            app.Run();
        }
    }
}
