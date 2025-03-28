using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SocietyMVC.Models;

namespace SocietyMVC.Repository
{
    public interface IAuth
    {
        void Register(UserModel user);
        IActionResult Login(string Email, string Password);
        void Logout();
        bool FlatExists(string flatNumber);
        bool EmailExists(string flatNumber);
        public List<UserModel> GetAllUsers();
        public void DeleteUser(int id);
        void UpdateUser(UserModel model);
        UserModel GetUserById(int id);
        void AddFlat(FlatModel flat);
    }
}
