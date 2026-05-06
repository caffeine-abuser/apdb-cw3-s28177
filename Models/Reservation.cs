public enum ReservationStatus
{
    PLANNED,
    CONFIRMED,
    CANCELLED
}

public class Reservation(Guid? id, ReservationDTO data)
{
    public Guid Id = id ?? new Guid();
    public Guid RoomId = data.RoomId;
    public string OrganizerName = data.OrganizerName;
    public string Topic = data.Topic;
    public DateOnly Date = data.Date;
    public DateTime StartTime = data.StartTime;
    public DateTime EndTime = data.EndTime;
    public ReservationStatus Status = data.Status;
}

public class ReservationDTO(Guid roomId, string organizerName, string topic, DateOnly date, DateTime startTime, DateTime endTime, ReservationStatus status)
{
    public Guid RoomId = roomId;
    public string OrganizerName = organizerName;
    public string Topic = topic;
    public DateOnly Date = date;
    public DateTime StartTime = startTime;
    public DateTime EndTime = endTime;
    public ReservationStatus Status = status;
}

public class ReservationFilterDTO(DateOnly date, ReservationStatus status, Guid roomId)
{
    public DateOnly date = date;
    public ReservationStatus status = status;
    public Guid RoomId = roomId;
}