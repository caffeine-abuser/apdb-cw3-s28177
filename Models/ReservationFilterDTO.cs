public class ReservationFilterDTO
{
    public DateOnly? Date { get; set; }
    public ReservationStatus? Status { get; set; }
    public string? RoomId { get; set; }

    public ReservationFilterDTO(DateOnly date, ReservationStatus status, string roomId)
    {
        this.Date = date;
        this.Status = status;
        RoomId = roomId;
    }

    public ReservationFilterDTO()
    {
        Date = null;
        Status = null;
        RoomId = null;
    }
}