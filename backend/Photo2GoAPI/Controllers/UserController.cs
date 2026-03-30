using Microsoft.AspNetCore.Mvc;
using Photo2GoAPI.Model;
using Photo2GoAPI.Services;

namespace Photo2GoAPI.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] User request)
    {
        User? user = _userService.Login(request.Email, request.Password);

        if (user is null)
        {
            return Unauthorized("Neteisingi prisijungimo duomenys");
        }

        return Ok(user);
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] User request)
    {
        User? user = _userService.Register(request);

        if (user is null)
        {
            return BadRequest("Toks vartotojas jau yra");
        }

        return Ok(user);
    }
}
