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
        new Reservation(new Guid("08c5aaae-aa9c-476a-8da2-60696242f4d3"),
            new ReservationDTO(new Guid("4e162f7b-0ac6-4d1e-ab89-1eca2409815f"),
                "Jan Kowalski",
                "Administracja bazami danych",
                DateOnly.Parse("1994-01-01"),
                TimeOnly.Parse("11:00"),
                TimeOnly.Parse("12:30"),
                ReservationStatus.CONFIRMED)),
        // future reservation
        new Reservation(new Guid("58d42321-3784-4678-957a-f0a6b1a99588"),
            new ReservationDTO(new Guid("2a4a3437-fd5a-4007-b43b-306861fb65f0"),
                "Anna Kowalska",
                "Elektronika",
                DateOnly.Parse("2020-03-16"),
                TimeOnly.Parse("09:00"),
                TimeOnly.Parse("10:30"),
                ReservationStatus.PLANNED)),
        // future, cancelled reservation
        new Reservation(new Guid("08c5aaae-aa9c-476a-8da2-60696242f4d3"),
            new ReservationDTO(new Guid("4e162f7b-0ac6-4d1e-ab89-1eca2409815f"),
                "Jan Kowalski",
                "Administracja bazami danych",
                DateOnly.Parse("2037-04-28"),
                TimeOnly.Parse("11:00"),
                TimeOnly.Parse("12:30"),
                ReservationStatus.CANCELLED)),
        // normal reservations in various states
        new Reservation(new Guid("08c5aaae-aa9c-476a-8da2-60696242f4d3"),
            new ReservationDTO(new Guid("4e162f7b-0ac6-4d1e-ab89-1eca2409815f"),
                "Jan Kowalski",
                "Java 1.8 (nadal w 2026!)",
                DateOnly.Parse("2026-04-08"),
                TimeOnly.Parse("11:00"),
                TimeOnly.Parse("12:30"),
                ReservationStatus.PLANNED)),
        new Reservation(new Guid("2a4a3437-fd5a-4007-b43b-306861fb65f0"),
            new ReservationDTO(new Guid("4e162f7b-0ac6-4d1e-ab89-1eca2409815f"),
                "Hans Acker",
                "O sieciach jak ser szwajcarski: historie bitewne",
                DateOnly.Parse("1994-01-01"),
                TimeOnly.Parse("11:00"),
                TimeOnly.Parse("12:30"),
                ReservationStatus.CONFIRMED))
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

        _logger.LogDebug("valid room: {}, valid date: {} valid times: {}", isValidRoom, isValidDate, hasValidTimes);
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
        var filtered = storedReservations.Where(r => r.Id == id);

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
        if (!IsValidReservation(data)) {
            _logger.LogDebug("abandoning create, invalid reservation"); 
            return BadRequest();
        }

        if (string.IsNullOrWhiteSpace(data.OrganizerName) || string.IsNullOrWhiteSpace(data.Topic))
        {
            _logger.LogDebug("abandoning create, missing fields");
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
        var filtered = storedReservations.Where(r => r.Id == id);

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
        var updated = new Reservation(id, data);
        storedReservations.Add(updated);

        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteReservation(Guid id)
    {
        var filtered = storedReservations.Where(r => r.Id == id);

        if (!filtered.Any())
        {
            return NotFound("No room found with that ID.");
        }

        // other routes are highly defensive against duplicate UUIDs;
        // this route is highly *aggressive* and will just delete all of those to resolve any data *model* integrity concerns
        // while causing some very real data integrity concerns

        if (filtered.Count() > 1) _logger.LogWarning("data model integrity violated; deleting multiple room entries with UUID {RecordId}", filtered.First().Id);

        foreach (var room in filtered.ToList())
        {
            storedReservations.Remove(room);
        }

        return NoContent();
    }
}