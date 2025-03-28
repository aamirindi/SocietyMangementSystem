using Microsoft.EntityFrameworkCore;
using SocietyMVC.Data;
using SocietyMVC.Models;
using SocietyMVC.Repository;
using System.Net.Mail;

namespace SocietyMVC.Services
{
    public class VisitorService : IVisitor
    {
        private readonly ApplicatioDbContext db;

        public VisitorService(ApplicatioDbContext db)
        {
            this.db = db;
        }

        public async Task<int> RequestVisitorOTP(VisitorsModel visitor)
        {
            visitor.EntryTime = DateTime.Now;
            visitor.HasVehicle = false;
            visitor.SlotNo = null;

            var user = db.Users.FirstOrDefault(x => x.FlatNumber == visitor.FlatNumber);
            if (user == null)
            {
                throw new Exception("Resident (User) with the given FlatNumber does not exist.");
            }

            visitor.UserId = user.UserId;
            visitor.Status = VisitorStatus.Pending;

            db.Visitors.Add(visitor);
            await db.SaveChangesAsync();

            await GenerateAndSendOTP(visitor.VisitorId);

            return visitor.VisitorId;
        }

        public async Task<string> GenerateAndSendOTP(int visitorId)
        {
            var visitor = await db.Visitors.FindAsync(visitorId);
            if (visitor == null) return null;

            var otp = new Random().Next(1000, 9999).ToString();
            visitor.SlotNo = otp;
            visitor.OtpExpiry = DateTime.Now.AddMinutes(2);
            db.Visitors.Update(visitor);
            await db.SaveChangesAsync();

            var resident = await db.Users.FindAsync(visitor.UserId);
            if (resident == null) return null;

            // Construct the email body with visitor details

            string emailBody = $@"
    <h2 style='color: green;'>Visitor OTP</h2>
    <p><strong style='color: red;'>Visitor Name:</strong> {visitor.Name}</p>
    <p><strong>Email:</strong> {visitor.Email}</p>
    <p><strong style='color: purple;'>Phone Number:</strong> {visitor.PhoneNumber}</p>
    <p><strong style='color: purple;'>Has Vehicle:</strong> {(visitor.HasVehicle ? "Yes" : "No")}</p>
    <p><strong style='color: blue;'>OTP:</strong> <b style='color: blue;'>{otp}</b></p>
    <p style='color: #00008B;'><em>This OTP expires in 2 minutes.</em></p>";

            await SendEmail(resident.Email, "Visitor OTP", emailBody);

            return otp;
        }

        public async Task<bool> VerifyOTP(string visitorEmail, string enteredOtp)
        {
            var visitor = await db.Visitors.FirstOrDefaultAsync(v => v.Email == visitorEmail);
            if (visitor == null || visitor.SlotNo != enteredOtp || visitor.OtpExpiry < DateTime.Now)
            {

                if (visitor != null)
                {
                    db.Visitors.Remove(visitor);
                    await db.SaveChangesAsync();
                }
                return false;
            }

            visitor.Status = VisitorStatus.Approved;
            visitor.SlotNo = null;
            db.Visitors.Update(visitor);
            await db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ResendOTP(string visitorEmail)
        {
            var visitor = await db.Visitors.FirstOrDefaultAsync(v => v.Email == visitorEmail);
            if (visitor == null) return false;

            await GenerateAndSendOTP(visitor.VisitorId);
            return true;
        }


        public async Task<bool> ApproveVisitor(int visitorId)
        {
            var visitor = await db.Visitors.FindAsync(visitorId);
            if (visitor == null) return false;

            visitor.Status = VisitorStatus.Approved;
            db.Visitors.Update(visitor);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectVisitor(int visitorId)
        {
            var visitor = await db.Visitors.FindAsync(visitorId);
            if (visitor == null) return false;

            visitor.Status = VisitorStatus.Rejected;
            db.Visitors.Update(visitor);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteVisitor(string visitorEmail)
        {
            var visitor = await db.Visitors.FirstOrDefaultAsync(v => v.Email == visitorEmail);
            if (visitor != null)
            {
                db.Visitors.Remove(visitor);
                await db.SaveChangesAsync();
                return true;
            }
            return false;
        }


        private async Task SendEmail(string toEmail, string subject, string body)
        {
            var smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new System.Net.NetworkCredential("", ""),
                EnableSsl = true
            };

            var message = new MailMessage
            {
                From = new MailAddress(""),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(toEmail);
            await smtp.SendMailAsync(message);
        }

        public void DeleteVisitorBtn(int id)
        {
            var visitor = db.Visitors.Find(id);
            if (visitor != null)
            {
                db.Visitors.Remove(visitor);
                db.SaveChanges();
            }
        }

        public void UpdateExitTime(int id)
        {
            var visitor = db.Visitors.Find(id);
            if (visitor != null)
            {
                visitor.ExitTime = DateTime.Now;
                db.SaveChanges();
            }
        }

        public void RejectedVisitor(int id)
        {
            var visitor = db.Visitors.Find(id);
            if (visitor != null)
            {
                visitor.Status = VisitorStatus.Rejected;
                db.Visitors.Update(visitor);
                db.SaveChanges();
            }
        }
    }
}
