using Photo2GoAPI.Data;

namespace Photo2GoAPI.Services;

public class UserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db)
    {
        _db = db;
    }

}