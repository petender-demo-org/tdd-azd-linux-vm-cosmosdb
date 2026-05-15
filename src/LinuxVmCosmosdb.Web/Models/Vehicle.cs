namespace LinuxVmCosmosdb.Web.Models;

public class Vehicle
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string PlateNumber { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = "Available";
    public string CurrentLocation { get; set; } = string.Empty;
    public double CapacityKg { get; set; }
    public string DriverName { get; set; } = string.Empty;
    public int MileageKm { get; set; }
    public DateTime LastMaintenanceDate { get; set; }
}
