using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocietyMVC.Data;
using SocietyMVC.Models;
using SocietyMVC.Repository;

namespace SocietyMVC.Controllers
{
    public class EventController : Controller
    {

        IEventServices em;
        public EventController(IEventServices em)
        {
            this.em = em;
        }



        public IActionResult Index()
        {
            var data = em.FetchAllEvents();
            return View(data);
        }

        public IActionResult CreateEvent()
        {
            return View();
        }


        [HttpPost]
        public IActionResult CreateEvent(EventMeetingModel e)
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString("MyUserId"));
            e.CreatedBy = userId;
            em.AddEvent(e);
            TempData["msg"] = "Event Created Successfully";
            return RedirectToAction("Index", "Event");
        }



        public IActionResult RsvpList()
        {

            var data = em.FetchAllRSVP();
            return View(data);
        }

        public IActionResult DeleteEvent(int id)
        {
            em.DeleteEventbById(id);
            TempData["delmsg"] = "Event deleted Successfully";
            return RedirectToAction("Index", "Event");
        }

        public IActionResult EditEvent(int id)
        {
            var data = em.FindEventById(id);
            return View(data);
        }

        [HttpPost]
        public IActionResult EditEvent(EventMeetingModel e)
        {
            em.UpdateEvent(e);
            TempData["Updmsg"] = "Event Updated Successfully";
            return RedirectToAction("Index", "Event");
        }

        public IActionResult UserRsvp()
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString("MyUserId"));
            var rsvpList = em.GetRSVPsByUserId(userId);
            return View(rsvpList);
        }

        //public IActionResult EditUserRSVP()
        //{
        //    // Assuming you're storing UserId in session or getting from User Claims
        //    int userId = Convert.ToInt32(HttpContext.Session.GetString("MyUserId"));
        //    var rsvpList = em.GetRSVPsByUserId(userId);
        //    return View(rsvpList);
        //}

        public IActionResult EditUserRSVP(int id)
        {
            var data = em.FindRSVPById(id);
            return View(data);
        }

        //[HttpPost]
        //public IActionResult EditUserRSVP(EventRSVPModel e)
        //{
        //    em.UpdateRsvp(e);
        //    TempData["Updmsg"] = "Event Updated Successfully";
        //    return RedirectToAction("UserRsvp", "Event");
        //}




    }
}
