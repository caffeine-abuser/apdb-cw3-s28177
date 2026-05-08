public class RoomFilterDTO
{
    public int? MinCapacity { get; set; }
    public bool? HasProjector { get; set; }
    public bool? IsActive { get; set; }

    public RoomFilterDTO(int? minCapacity, bool? hasProjector, bool? isActive)
    {
        MinCapacity = minCapacity;
        HasProjector = hasProjector;
        IsActive = isActive;
    }

    public RoomFilterDTO()
    {
        MinCapacity = null;
        HasProjector = null;
        IsActive = null;
    }
}