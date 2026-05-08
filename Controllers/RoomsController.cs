using Microsoft.AspNetCore.Components.Endpoints;
using Microsoft.AspNetCore.Mvc;

namespace apdb_cw3_s28177.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly ILogger<RoomsController> _logger;

    // quick stats:
    // 5 rooms, 3 active, 2 inactive, 2 with a projector
    public static List<Room> storedRooms = [
        new Room("4e162f7b-0ac6-4d1e-ab89-1eca2409815f", new RoomDTO("Pokój 031", "HQ-A", 0, 5, false, true)),
        new Room("e65b80c1-359a-48ce-bc28-9686c60ff4b9", new RoomDTO("Aula", "HQ-A", 1, 175, true, true)),
        new Room("2a4a3437-fd5a-4007-b43b-306861fb65f0", new RoomDTO("Pokój 114", "HQ-B", 1, 15, false, true)),
        new Room("9cc5ee87-7846-46ea-a96b-1f268ddc96cb", new RoomDTO("Pokój 503", "HQ-D", 5, 25, false, false)),
        new Room("281f8eaf-99af-4c5a-95bd-19506451ddd9", new RoomDTO("Pokój 207", "BRANCH-A", 2, 3, true, false))
    ];

    public RoomsController(ILogger<RoomsController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetRooms([FromQuery] RoomFilterDTO filters)
    {
        if (filters.MinCapacity == null && filters.HasProjector == null && filters.IsActive == null)
        {
            return Ok(storedRooms);
        }

        var filtered = storedRooms.AsQueryable();

        if (filters.MinCapacity != null) filtered = filtered.Where(r => r.Capacity >= filters.MinCapacity);
        if (filters.HasProjector != null) filtered = filtered.Where(r => r.HasProjector == filters.HasProjector);
        if (filters.IsActive != null) filtered = filtered.Where(r => r.IsActive == filters.IsActive);

        return Ok(filtered.ToList());
    }

    [HttpGet("{id}")]
    public IActionResult GetRoomById(Guid id)
    {
        var filtered = storedRooms.Where(r => r.Id == id.ToString());

        if (!filtered.Any())
        {
            return NotFound("No room found with that ID.");
        }
        // catch the weird case
        else if (filtered.Count() > 1)
        {
            _logger.LogWarning(">globally unique identifier");
            _logger.LogWarning(">look inside");
            _logger.LogWarning(">globally non-unique identifier");
        }

        return Ok(filtered.First());
    }

    [HttpGet("building/{BldgCode}")]
    public IActionResult GetRoomByBldgCode(string BldgCode)
    {
        var filtered = storedRooms.Where(r => BldgCode == r.BuildingCode);

        if (!filtered.Any()) { return NotFound("No such building code found."); }

        return Ok(filtered.ToList());
    }


    [HttpPost]
    public IActionResult CreateRoom(RoomDTO data)
    {
        if (String.IsNullOrWhiteSpace(data.Name) || String.IsNullOrWhiteSpace(data.BuildingCode))
        {
            return BadRequest();
        }

        var newRoom = new Room(null, data);
        storedRooms.Add(newRoom);
        return CreatedAtAction(nameof(GetRoomById), new { id = newRoom.Id }, newRoom);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateRoom(Guid id, RoomDTO room)
    {
        var filtered = storedRooms.Where(r => r.Id == id.ToString());

        if (!filtered.Any())
        {
            return NotFound("No room found with that ID.");
        }
        // catch the weird case
        else if (filtered.Count() > 1)
        {
            _logger.LogWarning(">globally unique identifier");
            _logger.LogWarning(">look inside");
            _logger.LogWarning(">globally non-unique identifier");
        }

        storedRooms.Remove(filtered.First());
        var updated = new Room(id.ToString(), room);
        storedRooms.Add(updated);

        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteRoom(Guid id)
    {
        var filtered = storedRooms.Where(r => r.Id == id.ToString());

        if (!filtered.Any())
        {
            return NotFound("No room found with that ID.");
        }

        foreach (var room in filtered)
        {
            var found = ReservationsController.storedReservations
                        .Any(res => res.RoomId == room.Id && res.Date >= DateOnly.FromDateTime(DateTime.Now) && res.Status != ReservationStatus.CANCELLED);

            if (found) return Conflict();
        }

        // other routes are highly defensive against duplicate UUIDs;
        // this route is highly *aggressive* and will just delete all of those to resolve any data *model* integrity concerns
        // while causing some very real data integrity concerns

        if (filtered.Count() > 1) _logger.LogWarning("data model integrity violated; deleting multiple room entries with UUID {RecordId}", filtered.First().Id);

        foreach (var room in filtered.ToList())
        {
            storedRooms.Remove(room);
        }

        return NoContent();
    }
}