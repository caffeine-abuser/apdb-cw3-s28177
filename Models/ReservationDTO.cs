using System.ComponentModel.DataAnnotations;

public class ReservationDTO
{
    [Required]
    public Guid RoomId { get; set; }
    [Required]
    public string OrganizerName { get; set; }
    [Required]
    public string Topic { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public ReservationStatus Status { get; set; }

    public ReservationDTO(Guid roomId, string organizerName, string topic, DateOnly date, TimeOnly startTime, TimeOnly endTime, ReservationStatus status)
    {
        RoomId = roomId;
        OrganizerName = organizerName;
        Topic = topic;
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
        Status = status;
    }
}
