using Microsoft.AspNetCore.Mvc;
using SocietyMVC.Data;
using SocietyMVC.Models;
using SocietyMVC.Repository;

namespace SocietyMVC.Controllers
{
    public class ComplainController : Controller
    {
        IComplain comp;
        public ComplainController(IComplain comp)
        {
            this.comp = comp;
        }

        public IActionResult AddComplaint()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddComplaint(ComplainModel c)
        {
            var Id = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            comp.AddComplain(c, Id);
            return RedirectToAction("ViewComplaint");

        }

        public IActionResult ViewComplaint(int id)
        {
            var data = comp.FetchComplain();
            return View(data);

        }

        public IActionResult DeleteComplain(int id)
        {
            comp.DeleteComplain(id);
            return RedirectToAction("ViewComplaint");

        }
        public IActionResult EditComplain(int id)
        {
            var data = comp.FetchDataById(id);

            return View(data);
        }
        [HttpPost]
        public IActionResult EditComplain(ComplainModel c)
        {
            comp.EditComplain(c);
            return RedirectToAction("ViewComplaint");
        }
        public IActionResult AdminComplainList()
        {
            var data = comp.FetchComplain();
            return View(data);
        }

        public IActionResult ResponseComplain(int id)
        {
            var data = comp.FetchDataById(id);
            return View(data);
        }
        [HttpPost]
        public IActionResult ResponseComplain(ComplainModel c)
        {
            comp.EditComplain(c);
            return RedirectToAction("AdminComplainList");
        }



    }
}