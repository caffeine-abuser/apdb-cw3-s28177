using System.ComponentModel.DataAnnotations;

public class Room(Guid? id, RoomDTO room)
{
    [Required]
    public Guid Id = id ?? new Guid();
    [Required]
    public string Name = room.Name;
    [Required]
    public string BuildingCode = room.BuildingCode;
    public int Floor = room.Floor;
    [Range(1, int.MaxValue)]
    public int Capacity = room.Capacity;
    public bool HasProjector = room.HasProjector;
    public bool IsActive = room.IsActive;
}

public class RoomDTO(string name, string buildingCode, int floor, int capacity, bool hasProjector, bool isActive)
{
    [Required]
    public string Name = name;
    [Required]
    public string BuildingCode = buildingCode;
    public int Floor = floor;
    [Range(1, int.MaxValue)]
    public int Capacity = capacity;
    public bool HasProjector = hasProjector;
    public bool IsActive = isActive;
}

public class RoomFilterDTO(int? minCapacity, bool? hasProjector, bool? isActive)
{
    public int? MinCapacity = minCapacity;
    public bool? HasProjector = hasProjector;
    public bool? IsActive = isActive;
}