using Microsoft.AspNetCore.Mvc;

namespace apdb_cw3_s28177.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly ILogger<RoomsController> _logger;
    public List<Room> storedRooms = [
        // TODO: add 4-5 rooms here
    ];

    public RoomsController(ILogger<RoomsController> logger)
    {
        _logger = logger;
    }


    [HttpGet]
    public IActionResult GetAllRooms()
    {
        return Ok(storedRooms);
    }

    [HttpGet("{id}")]
    public IActionResult GetRoomById(Guid id)
    {
        var filtered = storedRooms.Where(s => s.Id == id);

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

    [HttpGet]
    public IActionResult GetRoomByFilters([FromQuery] RoomFilterDTO filters)
    {
        if (filters.MinCapacity == null && filters.HasProjector == null && filters.IsActive == null)
        {
            return BadRequest("At least one filter query param must be provided.");
        }

        var filtered = storedRooms.AsQueryable();

        if (filters.MinCapacity != null)  filtered = filtered.Where(r => r.Capacity >= filters.MinCapacity);
        if (filters.HasProjector != null) filtered = filtered.Where(r => r.HasProjector == filters.HasProjector);
        if (filters.IsActive != null)     filtered = filtered.Where(r => r.IsActive == filters.IsActive);

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
        return CreatedAtAction(nameof(GetRoomById), new { id = newRoom.Id }, newRoom);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateRoom(Guid id, RoomDTO room)
    {
        var filtered = storedRooms.Where(r => r.Id == id);

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
        var updated = new Room(id, room);
        storedRooms.Add(updated);

        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteRoom(Guid id)
    {
        var filtered = storedRooms.Where(r => r.Id == id);
        
        if (!filtered.Any())
        {
            return NotFound("No room found with that ID.");
        }

        // TODO: disallow deletion if there are future reservations tied to the room
        // return 409 Conflict

        // other routes are highly defensive against duplicate UUIDs;
        // this route is highly *aggressive* and will just delete all of those to resolve any data *model* integrity concerns
        // while causing some very real data integrity concerns

        foreach (var room in filtered)
        {
            storedRooms.Remove(room);
        }

        return NoContent();
    }
}