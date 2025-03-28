using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SocietyMVC.Data;
using SocietyMVC.Models;
using SocietyMVC.Repository;
using System.Collections.Generic;
using System.Linq;

namespace SocietyMVC.Services
{
    public class ParkingService : IParking
    {
        private readonly ApplicatioDbContext db;
        private readonly IHttpContextAccessor httpContext;

        public ParkingService(ApplicatioDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            this.db = db;
            httpContext = httpContextAccessor;
        }
        public List<ParkingModel> GetAll()
        {
            return db.Parking
        .Select(p => new ParkingModel
        {
            SlotId = p.SlotId,
            UserId = p.UserId,  // ✅ Default to 0 if NULL
            VehicleNo = p.VehicleNo ?? "Not Assigned",  // ✅ Avoid NULL errors
            Status = p.Status ?? "Vacant",  // ✅ Default value
            VehicleCategory = p.VehicleCategory ?? "Unknown",
            OccupiedBy = p.OccupiedBy ?? "None",
            AssignedTime = p.AssignedTime,  // ✅ Nullable, no change needed
            SlotNumber = p.SlotNumber ?? "Not Assigned",
            ExitTime = p.ExitTime
        })
        .ToList();
        }

        // Fetch only users who have a registered vehicle
        public List<SelectListItem> GetUsersWithVehicles()
        {
            return db.Parking
                .Where(p => !string.IsNullOrEmpty(p.VehicleNo)) // Only users with a vehicle
                .Select(p => new SelectListItem
                {
                    Value = p.UserId.ToString(), // Store UserId
                    Text = db.Users
                        .Where(u => u.UserId == p.UserId)
                        .Select(u => u.FlatNumber)
                        .FirstOrDefault() // Display FlatNumber
                })
                .ToList();
        }

        public void DeleteParkingReq(int slotId)
        {
            var parking = db.Parking
      .Where(p => p.SlotId == slotId)
      .Select(p => new ParkingModel
      {
          SlotId = p.SlotId,
          UserId = p.UserId,  // Default to 0 if NULL
          VehicleNo = p.VehicleNo ?? "Not Assigned",
          Status = p.Status ?? "Vacant",
          VehicleCategory = p.VehicleCategory ?? "Unknown",
          OccupiedBy = p.OccupiedBy ?? "None",
          AssignedTime = p.AssignedTime,  // No change needed (Nullable DateTime)
          SlotNumber = p.SlotNumber ?? "Not Assigned",
          ExitTime = p.ExitTime  // No change needed (Nullable DateTime)
      })
      .FirstOrDefault();

            db.Parking.Remove(parking);
            db.SaveChanges();
        }

        // Add vehicle and request parking slot
        public string AddVehicle(ParkingModel pm)
        {
            // Retrieve UserId from Session
            var userIdStr = httpContext.HttpContext.Session.GetString("UserId");

            // Ensure UserId is not null or empty before parsing
            if (string.IsNullOrEmpty(userIdStr))
            {
                return "User session expired or invalid. Please log in again.";
            }

            int userId = int.Parse(userIdStr);
            pm.UserId = userId;

            // Check if the user already has an active parking slot
            // bool isOccupied = db.Parking.Any(p => p.UserId == userId && p.Status == "Occupied");
            bool isOccupied = db.Parking.Any(p => p.UserId == userId && (p.Status == "Occupied" || p.Status == "Vacant") && p.ExitTime == null);

            if (isOccupied)
            {
                return "You already have an active parking slot assigned. Cannot add another vehicle.";
            }

            // Allow adding a vehicle since the user has exited their last parking slot
            pm.Status = "Vacant";
            pm.SlotNumber = "N/A";
            db.Parking.Add(pm);
            db.SaveChanges();
            int Slotid = pm.SlotId;

            // Fetch the assigned Flat Number (if exists)
            var flatNumber = db.Users.Where(u => u.UserId == userId).Select(u => u.FlatNumber).FirstOrDefault();

            // Generate a notification for the user
            GenerateParkingNotification(pm.VehicleNo, flatNumber, Slotid);

            return "Vehicle added successfully. Waiting for parking slot assignment.";
        }

        public void GenerateParkingNotification(string vehicleNo, string flatNumber, int slotId)
        {
            // Fetch all admin users
            var adminUsers = db.Users.Where(u => u.Role == "Admin").ToList();

            foreach (var admin in adminUsers)
            {
                NotificationModel notification = new NotificationModel()
                {
                    UserId = admin.UserId,  // Send notification to admin users
                    Title = $"Vehicle {vehicleNo} Added Successfully for Flat {flatNumber}",
                    Billlink = $"{slotId}", // No specific link needed for parking
                    CreatedDate = DateTime.Now,
                    Status = 0 // Default unread notification
                };

                db.notification.Add(notification);
            }

            db.SaveChanges(); // Save all notifications at once
        }



        public ParkingModel FindBySlotId(int slotId)
        {
            return db.Parking
                     .Where(p => p.SlotId == slotId)
                     .Select(p => new ParkingModel
                     {
                         SlotId = p.SlotId,
                         UserId = p.UserId,
                         VehicleNo = p.VehicleNo,
                         Status = p.Status,
                         AssignedTime = p.AssignedTime,
                         OccupiedBy = "N/A",
                         SlotNumber = "N/A",
                         VehicleCategory = p.VehicleCategory,
                         ExitTime = p.ExitTime,
                         User = db.Users.Where(u => u.UserId == p.UserId)
                                .Select(u => new UserModel
                                {
                                    UserId = u.UserId,
                                    FlatNumber = u.FlatNumber,
                                    // Include other user properties as needed
                                })
                                .FirstOrDefault()
                     })
             .FirstOrDefault();
        }

        public void UpdateParking(ParkingModel pm)
        {

            var user = db.Users.FirstOrDefault(u => u.UserId == pm.UserId);
            if (user != null)
            {
                // Update the parking model with the necessary details
                pm.Status = "Occupied";
                pm.AssignedTime = DateTime.Now;
                pm.OccupiedBy = user.FlatNumber; // Set OccupiedBy to the user's FlatNumber

                // Update the parking entry in the database
                db.Parking.Update(pm);
                db.SaveChanges();
            }
            else
            {
                // Handle the case where the user is not found
                throw new Exception("User not found for the given UserId.");
            }
        }

        public List<SelectListItem> GetAvailableParkingSlots()
        {
            // Define all possible slots
            var allSlots = new List<string>
    {
        "A1", "A2", "A3", "A4", "A5",
        "B1", "B2", "B3", "B4", "B5"
    };

            // Get slots that are already occupied
            var assignedSlots = db.Parking
                                   .Where(p => p.Status == "Occupied")
                                   .Select(p => p.SlotNumber)
                                   .ToList();

            // Find available slots
            var availableSlots = allSlots.Except(assignedSlots).ToList();

            // Convert to SelectListItem for use in dropdown
            return availableSlots.Select(slot => new SelectListItem
            {
                Value = slot,
                Text = slot
            }).ToList();
        }


        public List<ParkingModel> findByUserId()
        {
            // Ensure session contains UserId before proceeding
            var userIdString = httpContext.HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                return null; // Handle cases where the user is not logged in
            }

            int userId = int.Parse(userIdString);

            return db.Parking
     .Where(p => p.UserId == userId)
     .Select(p => new ParkingModel
     {
         SlotId = p.SlotId,
         UserId = p.UserId,
         VehicleNo = p.VehicleNo ?? "N/A",
         Status = p.Status ?? "N/A",
         AssignedTime = p.AssignedTime, // Default to MinValue (Handle UI formatting)
         VehicleCategory = p.VehicleCategory ?? "N/A",
         OccupiedBy = p.OccupiedBy ?? "N/A",
         SlotNumber = p.SlotNumber ?? "N/A",
         ExitTime = p.ExitTime // Default to MinValue
     })
     .ToList();
        }


        public void ExitUser(int id)
        {
            var parkingSlot = db.Parking.FirstOrDefault(p => p.SlotId == id);

            if (parkingSlot != null)
            {
                parkingSlot.ExitTime = DateTime.Now;
                parkingSlot.Status = "Vacant";

                db.Parking.Update(parkingSlot);
                db.SaveChanges();
            }

        }



    }
}