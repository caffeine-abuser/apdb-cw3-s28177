public class Reservation
{
    public string Id { get; set; }
    public string RoomId { get; set; }
    public string OrganizerName { get; set; }
    public string Topic { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public ReservationStatus Status { get; set; }

    public Reservation(string? id, ReservationDTO data)
    {
        Id = id ?? Guid.NewGuid().ToString();
        RoomId = data.RoomId;
        OrganizerName = data.OrganizerName;
        Topic = data.Topic;
        Date = data.Date;
        StartTime = data.StartTime;
        EndTime = data.EndTime;
        Status = data.Status;
    }
}