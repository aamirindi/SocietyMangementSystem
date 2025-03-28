using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SocietyMVC.Models;
using SocietyMVC.Repository;
using SocietyMVC.Data;
using SocietyMVC.Services;
using iTextSharp.text.xml.simpleparser;

namespace SocietyMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuth auth;
        private readonly ApplicatioDbContext db;

        public AuthController(IAuth auth, ApplicatioDbContext db)
        {
            this.auth = auth;
            this.db = db;
        }
        public IActionResult Register()
        {
            var data = auth.GetAllUsers();
            var usm = new UserModel();
            var us = new UserViewModel
            {
                Users = data,
                NewUser = usm
            };
            return View(us);
        }

        [HttpPost]
        public IActionResult Register(UserViewModel u)
        {
            if (u.NewUser.Role == "Resident")
            {
                if (string.IsNullOrWhiteSpace(u.NewUser.FlatNumber))
                {
                    TempData["error"] = "Flat Number is required for Staff and Resident roles.";
                    return RedirectToAction("Register");
                }
            }
            else
            {
                u.NewUser.FlatNumber = null;
            }

            if (auth.FlatExists(u.NewUser.FlatNumber) && (u.NewUser.Role == "Resident"))
            {
                TempData["error"] = "Flat already exists";
                return RedirectToAction("Register");
            }
            if (auth.EmailExists(u.NewUser.Email))
            {
                TempData["error"] = "Email already exists";
                return RedirectToAction("Register");
            }


            auth.Register(u.NewUser);
            TempData["success"] = "Registered Successfully!";
            return RedirectToAction("Register");
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string Email, string Password)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            return auth.Login(Email, Password);
        }

        public IActionResult SignOut()
        {
            auth.Logout();
            return RedirectToAction("Login");
        }

        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result.Principal == null) return RedirectToAction("Login");

            var email = result.Principal.FindFirstValue(ClaimTypes.Email);
            var name = result.Principal.FindFirstValue(ClaimTypes.Name);

            var user = db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                TempData["error"] = "No account found. Please register first.";
                return RedirectToAction("Login");
            }

            HttpContext.Session.SetString("MyId", email);
            HttpContext.Session.SetString("MyRole", user.Role);
            HttpContext.Session.SetString("MyUser", user.Name);
            HttpContext.Session.SetString("UserId", user.UserId.ToString());

            TempData["success"] = $"Logged in as {name}";

            return user.Role switch
            {
                "Admin" => RedirectToAction("AdminDashboard", "Home"),
                "Resident" => RedirectToAction("ResidentDashboard", "Home"),
                "Staff" => RedirectToAction("StaffDashboard", "Home"),
                _ => RedirectToAction("Login"),
            };
        }

        public IActionResult ViewUser(int id)
        {
            var user = auth.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        public IActionResult DeleteUser(int id)
        {
            auth.DeleteUser(id);
            TempData["error"] = "User Deleted Successfully";
            return RedirectToAction("Register");
        }

        public IActionResult EditUser(int uid)
        {
            var data = db.Users.Find(uid);
            if (data == null)
            {
                return Json(new { success = false, message = "User not found" });
            }
            return Json(new
            {
                id = data.UserId,
                name = data.Name,
                email = data.Email,
                phoneNumber = data.PhoneNumber,
                flatNumber = data.FlatNumber,
                role = data.Role
            });
        }


        [HttpPost]
        public IActionResult UpdateUser([FromBody] UserModel u)
        {
            var user = db.Users.Find(u.UserId);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            user.Name = u.Name;
            user.Email = u.Email;
            user.PhoneNumber = u.PhoneNumber;
            user.FlatNumber = u.FlatNumber;
            user.Role = u.Role;

            db.SaveChanges();
            return Json(new { success = true, message = "User updated successfully" });
        }

        // flat
        public IActionResult Flat()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddFlat(FlatModel f)
        {
            return Json("");
        }

        // Profile

        public IActionResult Profile()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetUserProfile()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "User not logged in." });
            }

            var user = db.Users
                .Where(u => u.UserId == int.Parse(userId))
                .Select(u => new
                {
                    u.UserId,
                    u.Name,
                    u.Email,
                    u.PhoneNumber,
                    u.FlatNumber,
                    u.Role
                })
                .FirstOrDefault();

            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            return Json(new { success = true, data = user });
        }

        [HttpPost]
        public IActionResult UpdateUserProfile([FromBody] UserModel model)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId) || model.UserId != int.Parse(userId))
            {
                return Json(new { success = false, message = "Unauthorized request." });
            }

            var user = db.Users.Find(model.UserId);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            user.Name = model.Name;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            db.SaveChanges();
            return Json(new { success = true, message = "Profile updated successfully." });
        }
    }
}
