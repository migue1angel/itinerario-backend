namespace Domain.Entities;

public class Boat
{
    public Guid Id { get; init; }
    public required string Name { get; set; }
    public Guid BasePortId { get; set; }
    public Port BasePort { get; set; } = null!;
    public int Capacity { get; set; }
}
