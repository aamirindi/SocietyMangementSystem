using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Razorpay.Api;
using SocietyMVC.Data;
using SocietyMVC.Models;
using SocietyMVC.Repository;
using System.Net.Http;
using System.Net.Mail;
using System.Net;

namespace SocietyMVC.Services
{
    public class MaintenanceService : IMaintenance
    {
        private readonly ApplicatioDbContext db;
        private readonly IHttpContextAccessor httpContext;
        private readonly string razorpayKey = "rzp_test_Kl7588Yie2yJTV";
        private readonly string razorpaySecret = "6dN9Nqs7M6HPFMlL45AhaTgp";

        public MaintenanceService(ApplicatioDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            this.db = db;
            httpContext = httpContextAccessor;
        }

        public void DeleteBill(int id)
        {
            var main = db.Maintenance.Find(id);
            db.Maintenance.Remove(main);
            db.SaveChanges();
        }

        public List<SelectListItem> ViewGenrateBills()
        {
            return db.Users
                .Where(u => u.Role == "Resident")
                .Select(u => new SelectListItem
                {
                    Value = u.UserId.ToString(),
                    Text = u.FlatNumber
                })
                .ToList();
        }

        public void GenerateBill(MaintenanceModel m)
        {
            m.Status = "Pending";
            m.LateFeeApplied = false;
            string user = httpContext.HttpContext.Session.GetString("MyUser");
            m.IssuedBy = user;
            db.Maintenance.Add(m);
            db.SaveChanges();
            int billid = m.BillId;
            GenerateBillAddNotification(m.UserId, m.BillName, billid);
        }

        public void GenerateBillAddNotification(int id, string title, int billid)
        {
            NotificationModel notification = new NotificationModel()
            {
                UserId = id,
                Title = $"New {title} Added",
                Billlink = $"Maintenance/PayBill/{billid}",
                CreatedDate = DateTime.Now,
                Status = 0
            };

            db.notification.Add(notification);
            db.SaveChanges();
        }

        public List<MaintenanceModel> GetAll()
        {
            return db.Maintenance.Include(e => e.User).ToList();
        }

        public string FindFlatNumberByBillId(int id)
        {
            return db.Maintenance
                .Where(m => m.BillId == id)
                .Select(m => m.User.FlatNumber)
                .FirstOrDefault();
        }

        public MaintenanceModel findById(int id)
        {
            return db.Maintenance.Find(id);
        }

        public void UpdateBill(MaintenanceModel m)
        {
            db.Maintenance.Update(m);
            db.SaveChanges();
        }

        public List<MaintenanceModel> findByUserId()
        {
            int userId = int.Parse(httpContext.HttpContext.Session.GetString("UserId"));
            var maintenance = db.Maintenance.Include(m => m.User)
                       .Where(m => m.UserId == userId)
                       .ToList();
            return maintenance;
        }

        public string CreateOrder(int billId, double amount)
        {
            var client = new RazorpayClient(razorpayKey, razorpaySecret);
            Dictionary<string, object> options = new Dictionary<string, object>
            {
                { "amount", amount * 100 },
                { "currency", "INR" },
                { "receipt", "order_rcptid_" + billId },
                { "payment_capture", 1 }
            };

            Order order = client.Order.Create(options);
            return order["id"].ToString();
        }

        public MaintenanceModel PayBillById(int id)
        {
            return db.Maintenance
                     .Include(m => m.User)
                     .FirstOrDefault(m => m.BillId == id);
        }

        public bool VerifyPayment(string paymentId, string orderId, string signature)
        {
            try
            {
                var attributes = new Dictionary<string, string>
                {
                    { "razorpay_payment_id", paymentId },
                    { "razorpay_order_id", orderId },
                    { "razorpay_signature", signature }
                };

                Utils.verifyPaymentSignature(attributes);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public byte[] GenerateInvoicePdf(int billId)
        {
            var bill = db.Maintenance.Include(m => m.User).FirstOrDefault(m => m.BillId == billId);
            if (bill == null) throw new Exception("Bill not found!");

            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 30, 30, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                // Font Definitions
                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20, BaseColor.DARK_GRAY);
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.WHITE);
                Font normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                Font boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                Font smallFont = FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.GRAY);

                // Invoice Header
                Paragraph header = new Paragraph("MAINTENANCE INVOICE", titleFont);
                header.Alignment = Element.ALIGN_CENTER;
                header.SpacingAfter = 20f;
                document.Add(header);

                // Header Table
                PdfPTable headerTable = new PdfPTable(2);
                headerTable.WidthPercentage = 100;
                headerTable.DefaultCell.Border = Rectangle.NO_BORDER;

                // Left Column - Invoice Info
                PdfPCell leftCell = new PdfPCell();
                leftCell.Border = Rectangle.NO_BORDER;
                leftCell.AddElement(new Paragraph("Invoice #: " + bill.BillId, boldFont));
                leftCell.AddElement(new Paragraph("Issued On: " + DateTime.Now.ToString("dd-MMM-yyyy"), normalFont));
                leftCell.AddElement(new Paragraph("Due Date: " + bill.DueDate.ToString("dd-MMM-yyyy"), normalFont));
                leftCell.AddElement(new Paragraph("Status: " + bill.Status,
                    new Font(boldFont) { Color = bill.Status == "Paid" ? BaseColor.GREEN : BaseColor.RED }));

                // Right Column - Resident Info
                PdfPCell rightCell = new PdfPCell();
                rightCell.Border = Rectangle.NO_BORDER;
                rightCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                rightCell.AddElement(new Paragraph("BILL TO:", boldFont));
                rightCell.AddElement(new Paragraph(bill.User.Name, normalFont));
                rightCell.AddElement(new Paragraph("Flat #" + bill.User.FlatNumber, normalFont));
                rightCell.AddElement(new Paragraph(bill.User.Email, smallFont));

                headerTable.AddCell(leftCell);
                headerTable.AddCell(rightCell);
                document.Add(headerTable);

                document.Add(new Paragraph(" "));

                // Bill Details Table
                PdfPTable billTable = new PdfPTable(3);
                billTable.WidthPercentage = 100;
                billTable.SetWidths(new float[] { 60, 20, 20 });

                // Table Header
                PdfPCell cell = new PdfPCell(new Phrase("DESCRIPTION", headerFont));
                cell.BackgroundColor = new BaseColor(51, 51, 51);
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Padding = 8;
                billTable.AddCell(cell);

                cell = new PdfPCell(new Phrase("AMOUNT", headerFont));
                cell.BackgroundColor = new BaseColor(51, 51, 51);
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Padding = 8;
                billTable.AddCell(cell);

                cell = new PdfPCell(new Phrase("TOTAL", headerFont));
                cell.BackgroundColor = new BaseColor(51, 51, 51);
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Padding = 8;
                billTable.AddCell(cell);

                // Bill Item
                billTable.AddCell(new Phrase(bill.BillName, normalFont));
                billTable.AddCell(new Phrase("₹" + bill.Amount.ToString("0.00"), normalFont));
                billTable.AddCell(new Phrase("₹" + bill.Amount.ToString("0.00"), normalFont));

                // Add Late Fee if applicable
                if (bill.LateFeeApplied)
                {
                    billTable.AddCell(new Phrase("Late Fee", normalFont));
                    billTable.AddCell(new Phrase("₹50.00", normalFont));
                    billTable.AddCell(new Phrase("₹50.00", normalFont));
                }

                document.Add(billTable);

                // Total Amount Section
                PdfPTable totalTable = new PdfPTable(2);
                totalTable.WidthPercentage = 50;
                totalTable.HorizontalAlignment = Element.ALIGN_RIGHT;
                totalTable.DefaultCell.Border = Rectangle.NO_BORDER;

                totalTable.AddCell(new Phrase("Subtotal:", boldFont));
                totalTable.AddCell(new Phrase("₹" + bill.Amount.ToString("0.00"), normalFont));

                if (bill.LateFeeApplied)
                {
                    totalTable.AddCell(new Phrase("Late Fee:", boldFont));
                    totalTable.AddCell(new Phrase("₹50.00", normalFont));
                }

                totalTable.AddCell(new Phrase("Total Amount:", new Font(boldFont) { Size = 12 }));
                totalTable.AddCell(new Phrase("₹" + (bill.LateFeeApplied ? (bill.Amount + 50).ToString("0.00") : bill.Amount.ToString("0.00")),
                    new Font(normalFont) { Size = 12 }));

                document.Add(totalTable);

                // Payment Information
                Paragraph paymentInfo = new Paragraph();
                paymentInfo.Add(new Chunk("Payment Method: ", boldFont));
                paymentInfo.Add(new Chunk("Online Payment (Razorpay)", normalFont));
                paymentInfo.SpacingBefore = 20f;
                document.Add(paymentInfo);

                // Footer
                Paragraph footer = new Paragraph("Thank you for your payment!", new Font(normalFont));
                footer.Alignment = Element.ALIGN_CENTER;
                footer.SpacingBefore = 30f;
                document.Add(footer);

                // Terms and Conditions
                Paragraph terms = new Paragraph("Terms & Conditions:\n" +
                    "1. This is a computer generated invoice.\n" +
                    "2. Please keep this invoice for your records.", smallFont);
                terms.SpacingBefore = 20f;
                document.Add(terms);

                document.Close();
                return ms.ToArray();
            }
        }

        public void SendInvoicesEmail(int billid)
        {
            var bill = db.Maintenance.Include(m => m.User).FirstOrDefault(m => m.BillId == billid);
            byte[] pdfBytes = GenerateInvoicePdf(billid);
            string subject = "Payment Successful - Your Invoice";
            string body = $"{bill.User.Name},<br/><br/>Your Payment for <b>{bill.BillName}</b> has been received successfully.<br/>Please find the attached invoice.<br/><br/>Thank You!";
            var s = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("yadav1052004@gmail.com", "xmzypcywsgdwghjv"),
                EnableSsl = true
            };

            var MailMessage = new MailMessage
            {
                From = new MailAddress("yadav1052004@gmail.com"),
                IsBodyHtml = true,
                Subject = subject,
                Body = body
            };

            MailMessage.To.Add(bill.User.Email);
            if (pdfBytes != null)
            {
                Attachment attachment = new Attachment(new MemoryStream(pdfBytes), $"{bill.BillName}.pdf");
                MailMessage.Attachments.Add(attachment);
            }
            s.Send(MailMessage);
        }

        public List<MaintenanceModel> GetDueBills()
        {
            return db.Maintenance
                .Where(b => b.DueDate.Date == DateTime.Today && b.Status != "Paid" && b.Status != "Overdue")
                .ToList();
        }

        public void SendRemainderEmails()
        {
            var dueBills = GetDueBills();
            foreach (var bill in dueBills)
            {
                SendReminderEmail(bill.BillId);
            }
        }

        public void SendReminderEmail(int billId)
        {
            try
            {
                var bill = db.Maintenance.Include(m => m.User).FirstOrDefault(m => m.BillId == billId);
                if (bill == null) return;

                var notification = db.notification.FirstOrDefault(n => n.UserId == bill.UserId && n.Billlink != null);
                string billPaymentLink = notification?.Billlink ?? "#";

                string subject = "🔔 Payment Reminder: Last Day to Pay Your Bill!";
                string body = $@"
                    <h3>Dear {bill.User.Name},</h3>
                    <p>This is a reminder that your maintenance bill of ₹{bill.Amount} is due <b>today ({bill.DueDate:dd-MMM-yyyy})</b>.</p>
                    <p>To avoid a late fee of ₹50, please pay before the end of the day.</p>
                    <p><a href='{billPaymentLink}'>Click here to pay now</a></p>
                    <p>Thank you!</p>";

                var s = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("yadav1052004@gmail.com", "xmzypcywsgdwghjv"),
                    EnableSsl = true
                };

                var Mail = new MailMessage
                {
                    From = new MailAddress("yadav1052004@gmail.com"),
                    IsBodyHtml = true,
                    Subject = subject,
                    Body = body,
                };
                Mail.To.Add(bill.User.Email);

                s.Send(Mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
            }
        }
    }
}