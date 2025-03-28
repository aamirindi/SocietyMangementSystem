using Microsoft.AspNetCore.Mvc.Rendering;
using SocietyMVC.Models;
using System.Collections.Generic;

namespace SocietyMVC.Repository
{
    public interface IParking
    {
        public List<ParkingModel> GetAll();

        List<SelectListItem> GetUsersWithVehicles();
        string AddVehicle(ParkingModel pm);

        public ParkingModel FindBySlotId(int slotId);

        public void UpdateParking(ParkingModel pm);

        public List<SelectListItem> GetAvailableParkingSlots();

        public List<ParkingModel> findByUserId();

        public void ExitUser(int id);

        public void GenerateParkingNotification(string vehicleNo, string flatNumber, int slotId);

        public void DeleteParkingReq(int slotId);


    }
}