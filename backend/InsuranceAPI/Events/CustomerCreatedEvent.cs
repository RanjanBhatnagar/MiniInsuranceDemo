namespace InsuranceAPI.Events;

public class CustomerCreatedEvent
{
    public int CustomerId { get; set; }

    public string Name { get; set; } = "";

    public string Email { get; set; } = "";
}