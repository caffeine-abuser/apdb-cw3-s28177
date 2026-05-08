using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Room
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string BuildingCode { get; set; }
    public int Floor { get; set; }
    [Range(1, int.MaxValue)]
    public int Capacity { get; set; }
    public bool HasProjector { get; set; }
    public bool IsActive { get; set; }

    public Room(Guid? id, RoomDTO room)
    {
        Id = id ?? Guid.NewGuid();
        Name = room.Name;
        BuildingCode = room.BuildingCode;
        Floor = room.Floor;
        Capacity = room.Capacity;
        HasProjector = room.HasProjector;
        IsActive = room.IsActive;
    }
}
