using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocietyMVC.Data;
using SocietyMVC.Models;
using SocietyMVC.Repository;
using System.Text;

namespace SocietyMVC.Services
{
    public class AuthService : IAuth
    {
        private readonly ApplicatioDbContext db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(ApplicatioDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            this.db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Login(string Email, string Password)
        {
            var emailData = db.Users.FirstOrDefault(e => e.Email == Email);
            //var staffRole = db.Staff.FirstOrDefault(s => s.Role == )
            if (emailData != null)
            {
                var decryptPass = DecryptPassword(emailData.PasswordHash);
                if (decryptPass == Password)
                {
                    var httpContext = _httpContextAccessor.HttpContext;
                    string userEmail = emailData.Email;
                    string userName = emailData.Name;
                    int userId = emailData.UserId;

                    httpContext.Session.SetString("MyId", userEmail);

                    if (emailData.Role == "Admin")
                    {
                        httpContext.Session.SetString("MyRole", "Admin");
                        httpContext.Session.SetString("MyUser", userName);
                        httpContext.Session.SetString("MyId", userEmail);
                        httpContext.Session.SetString("UserId", userId.ToString());
                        httpContext.Session.SetString("MyUserId", userId.ToString());
                        return new RedirectToActionResult("AdminDashboard", "Home", null);
                    }
                    else if (emailData.Role == "Resident")
                    {
                        httpContext.Session.SetString("MyRole", "Resident");
                        httpContext.Session.SetString("MyUser", userName);
                        httpContext.Session.SetString("MyId", userEmail);
                        httpContext.Session.SetString("UserId", userId.ToString());
                        httpContext.Session.SetString("MyUserId", userId.ToString());
                        return new RedirectToActionResult("ResidentDashboard", "Home", null);
                    }
                    else if (emailData.Role == "Staff")
                    {
                        httpContext.Session.SetString("MyRole", "Staff");
                        httpContext.Session.SetString("MyUser", userName);
                        httpContext.Session.SetString("MyId", userEmail);
                        httpContext.Session.SetString("UserId", userId.ToString());
                        httpContext.Session.SetString("MyUserId", userId.ToString());
                        return new RedirectToActionResult("StaffDashboard", "Home", null);
                    }
                }
                else
                {
                    return new RedirectToActionResult("Login", "Auth", new { error = "Invalid Password" });
                }
            }
            return new RedirectToActionResult("Login", "Auth", new { error = "Invalid Email" });
        }

        public void Logout()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            httpContext.Session.Remove("MyId");
            httpContext.Session.Remove("MyRole");
            httpContext.Session.Remove("MyUser");
            httpContext.Session.Remove("UserId");
            httpContext.Session.Clear();
        }

        public void Register(UserModel user)
        {
            user.PasswordHash = EncryptPassword(user.PasswordHash);
            db.Users.Add(user);
            db.SaveChanges();
        }

        public bool FlatExists(string flatNumber)
        {
            return db.Users.Any(u => u.FlatNumber == flatNumber);
        }
        public bool EmailExists(string email)
        {
            return db.Users.Any(u => u.Email == email);
        }

        private static string DecryptPassword(string Password)
        {
            if (string.IsNullOrEmpty(Password))
            {
                return null;
            }
            byte[] data = Convert.FromBase64String(Password);
            return Encoding.UTF8.GetString(data);
        }

        private static string EncryptPassword(string Password)
        {
            if (string.IsNullOrEmpty(Password))
            {
                return null;
            }
            byte[] data = Encoding.UTF8.GetBytes(Password);
            return Convert.ToBase64String(data);
        }

        public List<UserModel> GetAllUsers()
        {
            var usersData = db.Users.ToList();

            if (usersData == null)
            {
                return new List<UserModel>();
            }

            return usersData;
        }

        public UserModel GetUserById(int id)
        {
            return db.Users.FirstOrDefault(u => u.UserId == id);
        }

        public void UpdateUser(UserModel model)
        {
            Console.WriteLine($"Updating user with ID: {model.UserId}");

            var existingUser = db.Users.FirstOrDefault(u => u.UserId == model.UserId);
            // var existingUser = db.Users.AsNoTracking().FirstOrDefault(u => u.UserId == model.UserId);

            if (existingUser == null)
            {
                Console.WriteLine($"User with ID {model.UserId} not found in database!");
                return;
            }

            existingUser.Name = model.Name;
            existingUser.Email = model.Email;
            existingUser.Role = model.Role;
            existingUser.PhoneNumber = model.PhoneNumber;
            existingUser.FlatNumber = model.FlatNumber;

            db.SaveChanges();
            Console.WriteLine("User updated successfully!");
        }

        public void DeleteUser(int id)
        {
            var data = db.Users.Find(id);
            db.Users.Remove(data);
            db.SaveChanges();
        }

        //  Flat 
        public void AddFlat(FlatModel flat)
        {
            db.Flats.Add(flat);
            db.SaveChanges();
        }
    }
}
