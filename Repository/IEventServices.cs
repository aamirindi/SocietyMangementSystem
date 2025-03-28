using SocietyMVC.Models;

namespace SocietyMVC.Repository
{
    public interface IEventServices
    {
         void AddEvent(EventMeetingModel em);
        
        List<EventMeetingModel> FetchAllEvents();

        void DeleteEventbById(int id);

        List<EventRSVPModel> FetchAllRSVP();

        EventMeetingModel FindEventById(int id);

        void UpdateEvent(EventMeetingModel em);

        List<EventRSVPModel> GetRSVPsByUserId(int userId);

        EventRSVPModel FindRSVPById(int id);

        void UpdateRsvp(EventRSVPModel em);
    }
}
