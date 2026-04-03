using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Photo2GoAPI.Data;
using Photo2GoAPI.Hubs;
using Photo2GoAPI.Model;
using Photo2GoAPI.Models;

namespace Photo2GoAPI.Controllers;

[ApiController]
[Route("api/routes")]
public class RoutesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IHubContext<NotificationHub> _hub;

    public RoutesController(AppDbContext db, IHubContext<NotificationHub> hub)
    {
        _db = db;
        _hub = hub;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RoutePlanDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var route = await _db.Routes
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (route is null)
        {
            return NotFound();
        }

        return Ok(ToDto(route));
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateRoutePlanRequest request,
        CancellationToken cancellationToken)
    {
        if (request.UserId <= 0)
        {
            return BadRequest(new { message = "UserId privalomas." });
        }

        var name = request.Name?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(name))
        {
            name = "Marsrutas";
        }

        var locationIds = request.LocationIds?
            .Where(id => id > 0)
            .Distinct()
            .ToArray() ?? Array.Empty<int>();

        if (locationIds.Length == 0)
        {
            return BadRequest(new { message = "LocationIds negali buti tuscias." });
        }

        var existingLocationIds = await _db.Locations
            .AsNoTracking()
            .Where(l => locationIds.Contains(l.Id))
            .Select(l => l.Id)
            .ToListAsync(cancellationToken);

        if (existingLocationIds.Count != locationIds.Length)
        {
            return BadRequest(new { message = "Vienas ar keli LocationIds neegzistuoja." });
        }

        var route = new RoutePlan
        {
            UserId = request.UserId,
            Name = name,
            CreatedAtUtc = DateTime.UtcNow
        };
        route.SetLocationIds(locationIds);

        _db.Routes.Add(route);
        await _db.SaveChangesAsync(cancellationToken);

        var responseMessage = "Marsrutas sekmingai sukurtas.";
        var routeDto = ToDto(route);

        var notification = new NotificationMessage
        {
            Type = "route_created",
            Message = responseMessage,
            Data = new { routeId = route.Id, name = route.Name },
            CreatedAtUtc = DateTime.UtcNow
        };

        // "Trigger": on successful route creation, push a realtime notification.
        await _hub.Clients
            .Group(NotificationHub.UserGroup(request.UserId))
            .SendAsync("notification", notification, cancellationToken);

        // Fallback: if the client isn't connected to SignalR yet, the HTTP response still has the message.
        return CreatedAtAction(
            nameof(GetById),
            new { id = route.Id },
            new CreateRoutePlanResponse
            {
                Message = responseMessage,
                Route = routeDto
            });
    }

    private static RoutePlanDto ToDto(RoutePlan route) => new()
    {
        Id = route.Id,
        UserId = route.UserId,
        Name = route.Name,
        LocationIds = route.GetLocationIds(),
        CreatedAtUtc = route.CreatedAtUtc
    };
}

