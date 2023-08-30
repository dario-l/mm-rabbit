namespace Fast.Services.Drivers.DataAccess.Entities;

internal sealed class Driver
{
    public required long Id { get; set; }
    public required string Name { get; set; }
    public bool Available { get; set; }
}