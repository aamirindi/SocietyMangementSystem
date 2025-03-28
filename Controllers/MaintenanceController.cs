using Microsoft.AspNetCore.Mvc;
using SocietyMVC.Data;
using SocietyMVC.Models;
using SocietyMVC.Repository;

namespace SocietyMVC.Controllers
{
    public class MaintenanceController : Controller
    {
        private readonly ApplicatioDbContext db;

        private IMaintenance man;

        public MaintenanceController(ApplicatioDbContext db, IMaintenance man)
        {
            this.db = db;
            this.man = man;
        }



        public IActionResult Index()
        {

            return View(man.GetAll());
        }



        public IActionResult GenerateBill()
        {
            ViewBag.Flatnumber = man.ViewGenrateBills();

            return View();

        }

        [HttpPost]
        public IActionResult GenerateBill(MaintenanceModel m)
        {

            man.GenerateBill(m);
            TempData["success"] = "Bill Added Succesfully";
            return RedirectToAction("Index");

        }

        public IActionResult DeleteBill(int id)
        {
            man.DeleteBill(id);
            TempData["error"] = "Bill Deleted Successfully";
            return RedirectToAction("Index");
        }

        public IActionResult UpdateBill(int id)
        {
            var flatNumber = man.FindFlatNumberByBillId(id);
            ViewBag.FlatNumber = flatNumber;
            var main = man.findById(id);
            return View(main);
        }

        [HttpPost]
        public IActionResult UpdateBill(MaintenanceModel m)
        {
            man.UpdateBill(m);
            TempData["upd"] = "Bill Updated Successfully";
            return RedirectToAction("Index");
        }

        public IActionResult PayBill(int id)
        {
            var bill = man.PayBillById(id); // Call service layer method

            //var bill = man.findById(id);
            if (bill == null)
            {
                return NotFound();
            }

            if (bill.DueDate < DateTime.Now && !bill.LateFeeApplied)
            {
                bill.Amount += 50; // Add ₹50 late fee directly to the amount
                bill.LateFeeApplied = true; // Mark the late fee as applied
                man.UpdateBill(bill); // Save updated amount and late fee status
            }

            string orderId = man.CreateOrder(bill.BillId, bill.Amount);

            ViewBag.OrderId = orderId;
            ViewBag.RazorpayKey = "rzp_test_Kl7588Yie2yJTV";
            ViewBag.Amount = bill.Amount * 100; // Convert to paisa
            ViewBag.BillId = bill.BillId;
            ViewBag.BillName = bill.BillName;
            ViewBag.UserEmail = bill.User.Email;

            return View();
        }

        public IActionResult PaymentSuccess(string razorpay_payment_id, string razorpay_order_id, string razorpay_signature, int billId)
        {
            bool isVerified = man.VerifyPayment(razorpay_payment_id, razorpay_order_id, razorpay_signature);

            if (isVerified)
            {
                var bill = man.findById(billId);
                if (bill != null)
                {
                    if (bill.DueDate < DateTime.Now)
                    {
                        bill.Status = "Overdue";  // ✅ Mark as Overdue if past due date
                    }
                    else
                    {
                        bill.Status = "Paid";  // ✅ Otherwise, mark as Paid
                    }
                    // ✅ Mark as Paid
                    man.UpdateBill(bill);  // ✅ Save Changes
                }
                man.SendInvoicesEmail(billId);

                TempData["InvoiceId"] = billId; // ✅ Set Invoice ID

                return RedirectToAction("UserIndex"); // ✅ Redirect to UserIndex
            }
            else
            {
                return View("PaymentFailed");
            }
        }

        public IActionResult GenerateInvoice(int id)
        {
            var invoiceBytes = man.GenerateInvoicePdf(id); // Ensure this method returns a valid PDF

            if (invoiceBytes == null)
            {
                return NotFound("Invoice not found.");
            }

            return File(invoiceBytes, "application/pdf", $"Invoice_{id}.pdf");
        }

        public IActionResult DownloadInvoice(int id)
        {
            var bill = man.findById(id);
            if (bill == null || (bill.Status != "Paid" && bill.Status != "Overdue"))
            {
                return NotFound("Invoice not available for this bill.");
            }

            var pdfBytes = man.GenerateInvoicePdf(bill.BillId);
            return File(pdfBytes, "application/pdf", $"Invoice_{id}.pdf");
        }








        public IActionResult ViewBill(int id)
        {
            var ma = man.findById(id);
            return View(ma);
        }

        public IActionResult UserIndex()
        {
            var data = man.findByUserId();
            return View(data);
        }

        public IActionResult SendPaymentReminders()
        {
            man.SendRemainderEmails(); // ✅ Directly call new method
            TempData["reminder"] = "Reminder emails sent successfully.";
            return RedirectToAction("Index");
        }




    }
}