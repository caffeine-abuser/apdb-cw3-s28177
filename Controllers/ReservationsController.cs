using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace apdb_cw3_s28177.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly ILogger<ReservationsController> _logger;

    // NOTE: this gets extraordinarily ugly, sadly. this would look a bit better in a proper database.
    public static List<Reservation> storedReservations = [
        // past reservation
        new Reservation("08c5aaae-aa9c-476a-8da2-60696242f4d3", new ReservationDTO("4e162f7b-0ac6-4d1e-ab89-1eca2409815f", "Jan Kowalski", "Administracja bazami danych", DateOnly.Parse("1994-01-01"), TimeOnly.Parse("11:00"), TimeOnly.Parse("12:30"), ReservationStatus.CONFIRMED)),
        // future, cancelled reservation
        new Reservation("58d42321-3784-4678-957a-f0a6b1a99588", new ReservationDTO("2a4a3437-fd5a-4007-b43b-306861fb65f0")),
        // normal reservations
        new Reservation("d4563a74-e007-487f-a239-ac9d50cee48d", new ReservationDTO("")),
        new Reservation("3e619f68-e860-407d-86fa-ed3a70388de4", new ReservationDTO("")),
        new Reservation("357b82ad-d2b3-44b2-aa92-73bd69273a3d", new ReservationDTO("")),
        new Reservation("e9396fcc-9ae7-4955-8a68-9d78dd86bd87", new ReservationDTO(""))
    ];

    public ReservationsController(ILogger<ReservationsController> logger)
    {
        _logger = logger;
    }

    private bool IsValidReservation(ReservationDTO reservation)
    {
        // basic checks
        // #1: check if the room id exists and is active
        // #2: check if the reservation isn't backdated
        // #3: check if the start-end times are ordered correctly

        var isValidRoom = RoomsController.storedRooms.Any(r => r.Id == reservation.RoomId && r.IsActive);
        var isValidDate = reservation.Date >= DateOnly.FromDateTime(DateTime.Now);
        var hasValidTimes = reservation.StartTime < reservation.EndTime;

        return isValidRoom && isValidDate && hasValidTimes;
    }

    [HttpGet]
    public IActionResult GetReservations([FromQuery] ReservationFilterDTO filters)
    {

        if (filters.RoomId == null && filters.Status == null && filters.Date == null)
        {
            return Ok(storedReservations);
        }

        var filtered = storedReservations.AsQueryable();

        if (filters.RoomId != null) filtered = filtered.Where(r => r.RoomId == filters.RoomId);
        if (filters.Status != null) filtered = filtered.Where(r => r.Status == filters.Status);
        if (filters.Date != null) filtered = filtered.Where(r => r.Date == filters.Date);

        return Ok(filtered.ToList());
    }

    [HttpGet("{id}")]
    public IActionResult GetReservationById(Guid id)
    {
        var filtered = storedReservations.Where(r => r.Id == id.ToString());

        if (!filtered.Any()) return NotFound("no reservation with such UUID.");
        else if (filtered.Count() > 1)
        {
            _logger.LogWarning(">globally unique identifier");
            _logger.LogWarning(">look inside");
            _logger.LogWarning(">globally non-unique identifier {uuid}", id);
        }

        return Ok(filtered.First());
    }

    [HttpPost]
    public IActionResult CreateReservation(ReservationDTO data)
    {
        if (!IsValidReservation(data)) return BadRequest();

        if (String.IsNullOrWhiteSpace(data.OrganizerName) || String.IsNullOrWhiteSpace(data.Topic))
        {
            return BadRequest();
        }

        // check if there is an overlap with any other reservations
        var overlaps = storedReservations.Where(r => r.Date == data.Date
                                                    && r.StartTime < data.EndTime
                                                    && data.StartTime < r.EndTime);

        if (overlaps.Any())
        {
            // return conflicting reservation IDs
            return Conflict(overlaps.Select(r => r.Id).ToList());
        }

        var newRes = new Reservation(null, data);
        storedReservations.Add(newRes);
        return CreatedAtAction(nameof(GetReservationById), new { id = newRes.Id }, newRes);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateReservation(Guid id, ReservationDTO data)
    {
        var filtered = storedReservations.Where(r => r.Id == id.ToString());

        if (!filtered.Any())
        {
            return NotFound("No reservation found with that ID.");
        }
        // catch the weird case
        else if (filtered.Count() > 1)
        {
            _logger.LogWarning(">globally unique identifier");
            _logger.LogWarning(">look inside");
            _logger.LogWarning(">globally non-unique identifier {uuid}", id);
        }

        if (!IsValidReservation(data)) return BadRequest();

        storedReservations.Remove(filtered.First());
        var updated = new Reservation(id.ToString(), data);
        storedReservations.Add(updated);

        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteReservation(Guid id)
    {
        return NotFound();
    }
}