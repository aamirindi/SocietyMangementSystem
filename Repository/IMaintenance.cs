using Microsoft.AspNetCore.Mvc.Rendering;
using SocietyMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace SocietyMVC.Repository
{
    public interface IMaintenance
    {
        public void GenerateBill(MaintenanceModel m);
        public void DeleteBill(int id);

        public List<MaintenanceModel> GetAll();

        public List<SelectListItem> ViewGenrateBills();

        public string FindFlatNumberByBillId(int id);

        public MaintenanceModel findById(int id);

        public void UpdateBill(MaintenanceModel m);

        public void GenerateBillAddNotification(int id, string title, int billid);

        public List<MaintenanceModel> findByUserId();

        string CreateOrder(int billId, double amount);
        bool VerifyPayment(string paymentId, string orderId, string signature);

        byte[] GenerateInvoicePdf(int billId);
        public MaintenanceModel PayBillById(int id);

        public void SendInvoicesEmail(int billid);

        public List<MaintenanceModel> GetDueBills();

        public void SendRemainderEmails();

    }
}