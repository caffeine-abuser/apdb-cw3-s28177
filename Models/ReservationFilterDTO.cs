public class ReservationFilterDTO
{
    public DateOnly? Date { get; set; }
    public ReservationStatus? Status { get; set; }
    public Guid? RoomId { get; set; }

    public ReservationFilterDTO(DateOnly date, ReservationStatus status, Guid roomId)
    {
        Date = date;
        Status = status;
        RoomId = roomId;
    }

    public ReservationFilterDTO()
    {
        Date = null;
        Status = null;
        RoomId = null;
    }
}