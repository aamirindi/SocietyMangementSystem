using Microsoft.EntityFrameworkCore;
using SocietyMVC.Data;
using SocietyMVC.Repository;
using SocietyMVC.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Hangfire;
using SocietyMVC.Service;

namespace SocietyMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<ApplicatioDbContext>
            (
                 options => options.UseSqlServer
                 (
                     builder.Configuration.GetConnectionString("dbconn")
                 )
             );

            // Session
            builder.Services.AddSession
                (
                    option =>
                    {
                        option.IdleTimeout = TimeSpan.FromMinutes(5);
                        option.Cookie.HttpOnly = true;
                        option.Cookie.IsEssential = true;

                    }
                );

            builder.Services.AddHttpContextAccessor();

            // Google Auth
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddGoogle(options =>
            {
                options.ClientId = "440780078459-0rfvhc05q86rpohfqh1lnbhbu021rnho.apps.googleusercontent.com";
                options.ClientSecret = "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";
            });

            // HttpContextAccessor
            builder.Services.AddScoped<IAuth, AuthService>();
            builder.Services.AddScoped<IMaintenance, MaintenanceService>();
            builder.Services.AddScoped<IVisitor, VisitorService>();
            builder.Services.AddScoped<IParking, ParkingService>();
            builder.Services.AddScoped<IComplain, Complain>();
            builder.Services.AddScoped<IEventServices, EventServices>();
            builder.Services.AddScoped<IStaff, StaffService>();
            builder.Services.AddScoped<INotice, NoticeService>();
            builder.Services.AddScoped<IStaffScheduleRepository, StaffScheduleService>();
            builder.Services.AddScoped<StaffSalaryService>();



            // ✅ Correctly Register Hangfire Before builder.Build()
            builder.Services.AddHangfire(x => x.UseSqlServerStorage(builder.Configuration.GetConnectionString("dbconn")));
            builder.Services.AddHangfireServer();

            var app = builder.Build();


            // ✅ Ensure Hangfire is Properly Configured
            app.UseHangfireDashboard("/hangfire");
            app.UseHangfireServer();

            using (var scope = app.Services.CreateScope())
            {
                var MaintenanceService = scope.ServiceProvider.GetRequiredService<IMaintenance>();

                RecurringJob.AddOrUpdate("reminderEmails",
                    () => MaintenanceService.SendRemainderEmails(), Cron.Daily(5, 25)); // ✅ Call new method
            }


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
            app.UseSession();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Auth}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
