public enum ReservationStatus
{
    PLANNED,
    CONFIRMED,
    CANCELLED
}

public class Reservation(int id, int roomId, string organizerName, string topic, DateOnly date, DateTime startTime, DateTime endTime, ReservationStatus status)
{
    int Id = id;
    int RoomId = roomId;
    string OrganizerName = organizerName;
    string Topic = topic;
    DateOnly Date = date;
    DateTime StartTime = startTime;
    DateTime EndTime = endTime;
    ReservationStatus Status = status;
}