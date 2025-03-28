using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocietyMVC.Data;
using SocietyMVC.Models;
using SocietyMVC.Repository;

namespace SocietyMVC.Services
{
    public class EventServices : IEventServices
    {
        private readonly ApplicatioDbContext db;
        public EventServices(ApplicatioDbContext db)
        {
            this.db = db;
        }

        public void AddEvent(EventMeetingModel em)
        {
            // Ensure the user creating the event exists
            var userExists = db.Users.Any(u => u.UserId == em.CreatedBy);
            if (!userExists)
            {
                throw new Exception("The user creating this event does not exist.");
            }

            // Add the event
            db.EventMeeting.Add(em);
            db.SaveChanges(); // Generates EventId (auto increment)

            // Fetch all Residents or specific users for RSVP and notification
            var residents = db.Users.Where(u => u.Role == "Resident").ToList();

            // Auto-create RSVP entries and Notifications
            foreach (var resident in residents)
            {
                // Create RSVP entry
                var rsvp = new EventRSVPModel
                {
                    EventId = em.EventId,
                    UserId = resident.UserId,
                    Response = "Maybe", // Default Response
                    RespondedOn = DateTime.Now
                };
                db.EventRSVP.Add(rsvp);

                // Create Notification entry
                var notify = new NotificationModel
                {
                    Title = $"New Event: {em.Title}",
                    Status = 0, // 0 = Unread
                    UserId = resident.UserId, // Resident who should be notified
                    Billlink = $"/Event/ViewEvent/{em.EventId}", // Optional: Add link if needed
                    CreatedDate = DateTime.Now
                };
                db.notification.Add(notify);
            }

            db.SaveChanges(); // Save RSVP and Notification entries

        }


        public List<EventMeetingModel> FetchAllEvents()
        {
            var data = db.EventMeeting
                         .Include(e => e.CreatedUser) // Important: Loads the CreatedBy User details
                         .ToList();
            return data;
        }


        public void DeleteEventbById(int id)
        {
            var data = db.EventMeeting.Find(id);
            if(data != null)
            {
                db.EventMeeting.Remove(data);
                db.SaveChanges();
            }
            
        }

        public List<EventRSVPModel> FetchAllRSVP()
        {
            var data = db.EventRSVP
                 .Include(r => r.User)
                 .Include(r => r.Event)
                 .ToList();
            return data;
        }

        public EventMeetingModel FindEventById(int id)
        {
            var data =db.EventMeeting.Find(id);
            if(data != null)
            {
                return data;
            }
            else
            {
                return null;
            }
        }

        public void UpdateEvent(EventMeetingModel em)
        {
            var existingEvent = db.EventMeeting.Find(em.EventId);
            if (existingEvent == null)
            {
                throw new Exception("Event not found");
            }

            // Optional: Keep CreatedBy intact
            em.CreatedBy = existingEvent.CreatedBy;

            db.Entry(existingEvent).CurrentValues.SetValues(em);
            db.SaveChanges();
        }

        public List<EventRSVPModel> GetRSVPsByUserId(int userId)
        {
             return db.EventRSVP
                  .Include(r => r.Event)  // Optional: load related Event info
                  .Where(r => r.UserId == userId)
                  .ToList();
        }

        public EventRSVPModel FindRSVPById(int id)
        {
            var data = db.EventRSVP.Find(id);
            if (data != null)
            {
                return data;
            }
            else
            {
                return null;
            }
        }

        public void UpdateRsvp(EventRSVPModel em)
        {
            db.EventRSVP.Update(em);
        }
    }

}
