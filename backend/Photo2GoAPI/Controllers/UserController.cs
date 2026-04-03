using Microsoft.AspNetCore.Mvc;
using Photo2GoAPI.Model;
using Photo2GoAPI.Services;

namespace Photo2GoAPI.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    private readonly GeneratedRouteService _generatedRouteService;

    public UserController(UserService userService, GeneratedRouteService generatedRouteService)
    {
        _userService = userService;
        _generatedRouteService = generatedRouteService;
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

    [HttpGet("{userId:int}/routes")]
    public async Task<IActionResult> GetRoutes(int userId, CancellationToken cancellationToken)
    {
        if (!await _generatedRouteService.UserExistsAsync(userId, cancellationToken))
        {
            return NotFound("Vartotojas nerastas");
        }

        var routes = await _generatedRouteService.GetByUserIdAsync(userId, cancellationToken);
        return Ok(routes);
    }
}
