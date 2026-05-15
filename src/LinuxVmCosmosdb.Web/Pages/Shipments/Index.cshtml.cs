using LinuxVmCosmosdb.Web.Models;
using LinuxVmCosmosdb.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LinuxVmCosmosdb.Web.Pages.Shipments;

public class IndexModel : PageModel
{
    private readonly CosmosDbService _cosmosDbService;

    public IndexModel(CosmosDbService cosmosDbService)
    {
        _cosmosDbService = cosmosDbService;
    }

    public List<Shipment> Shipments { get; set; } = [];

    public async Task OnGetAsync()
    {
        Shipments = await _cosmosDbService.GetShipmentsAsync();
    }

    public string GetStatusBadge(string status) => status switch
    {
        "InTransit" => "text-bg-warning",
        "Delivered" => "text-bg-success",
        "Pending" => "text-bg-secondary",
        _ => "text-bg-light"
    };
}
