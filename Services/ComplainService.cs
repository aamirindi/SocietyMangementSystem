using Microsoft.EntityFrameworkCore;
using SocietyMVC.Data;
using SocietyMVC.Models;
using SocietyMVC.Repository;
using System.Security.Cryptography;

namespace SocietyMVC.Service
{
    public class Complain : IComplain
    {
        private readonly ApplicatioDbContext db;
        public Complain(ApplicatioDbContext db)
        {
            this.db = db;
        }
        void IComplain.AddComplain(ComplainModel c, int Id)
        {
            c.UserId = Id;
            db.Complain.Add(c);
            db.SaveChanges();
        }

        public List<ComplainModel> FetchComplain()
        {
            var data = db.Complain.ToList();
            return data;

        }

        void IComplain.DeleteComplain(int cid)
        {
            var data = db.Complain.Find(cid);
            if (data != null)
            {
                db.Complain.Remove(data);
                db.SaveChanges();
            }
        }

        public ComplainModel FetchDataById(int id)
        {
            var data = db.Complain.Find(id);
            return data;

        }

        public void EditComplain(ComplainModel c)
        {
            //c.UserId = 1;
            db.Complain.Update(c);
            db.SaveChanges();
        }
    }
}
