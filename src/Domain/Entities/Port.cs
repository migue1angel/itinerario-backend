namespace Domain.Entities;

public class Port 
{
    public Guid Id { get; init; }
    public required string Name { get; set; }
    public required string Island { get; set; }
    public required string Timezone { get; set; }
}