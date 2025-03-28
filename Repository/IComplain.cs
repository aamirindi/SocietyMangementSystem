using SocietyMVC.Models;

namespace SocietyMVC.Repository
{
    public interface IComplain
    {
        void AddComplain(ComplainModel c, int Id);
        List<ComplainModel> FetchComplain();
        void EditComplain(ComplainModel c);
        void DeleteComplain(int cid);

        ComplainModel FetchDataById(int id);
    }
}
