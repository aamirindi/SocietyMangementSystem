using SocietyMVC.Models;

namespace SocietyMVC.Repository
{
    public interface IVisitor
    {
        Task<int> RequestVisitorOTP(VisitorsModel visitor);
        Task<string> GenerateAndSendOTP(int visitorId);
        Task<bool> VerifyOTP(string visitorEmail, string enteredOtp);
        Task<bool> ResendOTP(string visitorEmail);
        Task<bool> ApproveVisitor(int visitorId);
        Task<bool> RejectVisitor(int visitorId);
        Task<bool> DeleteVisitor(string visitorEmail);
        public void DeleteVisitorBtn(int id);
        public void UpdateExitTime(int id);
        public void RejectedVisitor(int id);
    }
}
