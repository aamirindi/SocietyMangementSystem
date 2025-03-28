using SocietyMVC.Models;

public class UserViewModel
{
    public UserModel? NewUser { get; set; }
    public IEnumerable<UserModel>? Users { get; set; } = new List<UserModel>();
}
