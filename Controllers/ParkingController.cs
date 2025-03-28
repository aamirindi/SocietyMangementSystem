using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SocietyMVC.Models;
using SocietyMVC.Repository;
using SocietyMVC.Services;

namespace SocietyMVC.Controllers
{
    public class ParkingController : Controller
    {
        private readonly IParking ip;

        public ParkingController(IParking ip)
        {
            this.ip = ip;
        }

        public IActionResult Index()
        {

            return View(ip.GetAll());
        }
        // User adds vehicle and requests parking
        public IActionResult AddVehicle()
        {
            ViewBag.UsersWithVehicles = ip.GetUsersWithVehicles();
            return View();
        }

        [HttpPost]
        public IActionResult AddVehicle(ParkingModel model)
        {
            string message = ip.AddVehicle(model);

            if (message.Contains("already has a parking slot"))
                TempData["error"] = message;
            else
                TempData["success"] = message;

            return RedirectToAction("UserIndex");
        }

        public IActionResult UpdateParking(int id)
        {
            var parking = ip.FindBySlotId(id);
            if (parking != null)
            {
                // Get available parking slots from the service
                ViewBag.ParkingSlots = ip.GetAvailableParkingSlots();
                return View(parking);
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult UpdateParking(ParkingModel pm)
        {
            ip.UpdateParking(pm);
            TempData["success"] = "Parking slot updated successfully";
            return RedirectToAction("Index");
        }


        public IActionResult UserIndex()
        {
            var data = ip.findByUserId();
            return View(data);
        }

        public IActionResult Exit(int id)
        {
            ip.ExitUser(id);
            return RedirectToAction("UserIndex");
        }

        public IActionResult Delete(int id)
        {
            ip.DeleteParkingReq(id);
            TempData["success"] = "Request Delete Succesfully";
            return RedirectToAction("UserIndex");
        }
    }

}