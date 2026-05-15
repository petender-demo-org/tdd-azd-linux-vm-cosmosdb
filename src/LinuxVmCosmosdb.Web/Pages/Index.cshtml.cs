using LinuxVmCosmosdb.Web.Models;
using LinuxVmCosmosdb.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LinuxVmCosmosdb.Web.Pages;

public class IndexModel : PageModel
{
    private readonly CosmosDbService _cosmosDbService;

    public IndexModel(CosmosDbService cosmosDbService)
    {
        _cosmosDbService = cosmosDbService;
    }

    public List<Shipment> Shipments { get; set; } = [];
    public List<Vehicle> Vehicles { get; set; } = [];
    public int TotalShipments { get; set; }
    public int InTransitCount { get; set; }
    public int DeliveredCount { get; set; }
    public int PendingCount { get; set; }

    public async Task OnGetAsync()
    {
        Shipments = await _cosmosDbService.GetShipmentsAsync();
        Vehicles = await _cosmosDbService.GetVehiclesAsync();

        TotalShipments = Shipments.Count;
        InTransitCount = Shipments.Count(s => s.Status == "InTransit");
        DeliveredCount = Shipments.Count(s => s.Status == "Delivered");
        PendingCount = Shipments.Count(s => s.Status == "Pending");
    }

    public string GetStatusBadge(string status) => status switch
    {
        "InTransit" => "text-bg-warning",
        "Delivered" => "text-bg-success",
        "Pending" => "text-bg-secondary",
        _ => "text-bg-light"
    };

    public string GetVehicleStatusBadge(string status) => status switch
    {
        "InTransit" => "text-bg-warning",
        "Available" => "text-bg-success",
        "Maintenance" => "text-bg-danger",
        _ => "text-bg-light"
    };
}
