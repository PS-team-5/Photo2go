using Photo2GoAPI.Data;
using Photo2GoAPI.Model;

namespace Photo2GoAPI.Services;

public class UserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db)
    {
        _db = db;
    }

    public User? Login(string email, string password)
    {
        foreach (var user in _db.Users)
        {
            if (user.Email == email && user.Password == password)
            {
                return user;
            }
        }

        return null;
    }

    public User? Register(User newUser)
    {
        foreach (var user in _db.Users)
        {
            if (user.Email == newUser.Email || user.Username == newUser.Username)
            {
                return null;
            }
        }

        _db.Users.Add(newUser);
        _db.SaveChanges();

        return newUser;
    }
}
