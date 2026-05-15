using LinuxVmCosmosdb.Web.Models;
using LinuxVmCosmosdb.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LinuxVmCosmosdb.Web.Pages.Shipments;

public class DetailsModel : PageModel
{
    private readonly CosmosDbService _cosmosDbService;

    public DetailsModel(CosmosDbService cosmosDbService)
    {
        _cosmosDbService = cosmosDbService;
    }

    public Shipment? Shipment { get; set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        Shipment = await _cosmosDbService.GetShipmentAsync(id);
        return Page();
    }
}
