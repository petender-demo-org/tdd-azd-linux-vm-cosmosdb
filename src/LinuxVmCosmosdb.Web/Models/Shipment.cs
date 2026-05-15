namespace LinuxVmCosmosdb.Web.Models;

public class Shipment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TrackingNumber { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public string VehicleId { get; set; } = string.Empty;
    public string RouteId { get; set; } = string.Empty;
    public double WeightKg { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeliveredAt { get; set; }
    public string CustomerName { get; set; } = string.Empty;
}
