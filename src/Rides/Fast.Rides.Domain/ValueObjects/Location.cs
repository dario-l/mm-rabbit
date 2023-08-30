using Fast.Rides.Domain.Exceptions;

namespace Fast.Rides.Domain.ValueObjects;

internal record Location
{
    public string Value { get; }

    public Location(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length is < 3 or > 100)
        {
            throw new InvalidLocationException();
        }
        
        Value = value;
    }
};