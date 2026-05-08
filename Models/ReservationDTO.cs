using System.ComponentModel.DataAnnotations;

public class ReservationDTO(string roomId, string organizerName, string topic, DateOnly date, TimeOnly startTime, TimeOnly endTime, ReservationStatus status)
{
    [Required]
    public string RoomId = roomId;
    [Required]
    public string OrganizerName = organizerName;
    [Required]
    public string Topic = topic;
    public DateOnly Date = date;
    public TimeOnly StartTime = startTime;
    public TimeOnly EndTime = endTime;
    public ReservationStatus Status = status;
}
