using Microsoft.AspNetCore.Mvc;

namespace apdb_cw3_s28177.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly ILogger<ReservationsController> _logger;
    public List<Reservation> storedReservations = [
        // TODO: populate with 4-6 test reservations
    ];

    public ReservationsController(ILogger<ReservationsController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetAllReservations()
    {
        
    }

    [HttpGet]
    public IActionResult GetReservationsByFilter([FromQuery] ReservationFilterDTO data)
    {
        
    }

    [HttpGet("{id}")]
    public IActionResult GetReservationById(Guid id)
    {
        
    }

    [HttpPost]
    public IActionResult CreateReservation(ReservationDTO data)
    {
        
    }

    [HttpPut("{id}")]
    public IActionResult UpdateReservation(Guid id, ReservationDTO data)
    {
        
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteReservation(Guid id)
    {
        
    }
}