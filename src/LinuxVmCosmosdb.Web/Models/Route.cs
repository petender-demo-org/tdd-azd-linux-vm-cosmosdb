namespace LinuxVmCosmosdb.Web.Models;

public class DeliveryRoute
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string StartCity { get; set; } = string.Empty;
    public string EndCity { get; set; } = string.Empty;
    public double DistanceKm { get; set; }
    public int EstimatedHours { get; set; }
    public List<string> Waypoints { get; set; } = [];
    public bool IsActive { get; set; } = true;
}
