// src/Domain/ValueObjects/Location.cs
using MyApp.Domain.Exceptions;
using System.Text.Json.Serialization;

namespace MyApp.Domain.ValueObjects;

public sealed record Location
{
    [JsonPropertyName("lat")] public double Latitude { get; init; }
    [JsonPropertyName("lng")] public double Longitude { get; init; }

    // کانستراکتور خصوصی برای Immutability
    private Location() { }

    // متد استاتیک برای ساخت + اعتبارسنجی
    public static Location Create(double latitude, double longitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new DomainException("Latitude must be between -90 and 90.");
        if (longitude < -180 || longitude > 180)
            throw new DomainException("Longitude must be between -180 and 180.");

        return new Location
        {
            Latitude = latitude,
            Longitude = longitude
        };
    }

    // برای نمایش در Google Maps
    public string ToUrl() => $"https://www.google.com/maps?q={Latitude},{Longitude}";

    public override string ToString() => $"{Latitude},{Longitude}";
}