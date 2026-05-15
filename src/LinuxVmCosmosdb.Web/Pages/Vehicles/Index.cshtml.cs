using LinuxVmCosmosdb.Web.Models;
using LinuxVmCosmosdb.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LinuxVmCosmosdb.Web.Pages.Vehicles;

public class IndexModel : PageModel
{
    private readonly CosmosDbService _cosmosDbService;

    public IndexModel(CosmosDbService cosmosDbService)
    {
        _cosmosDbService = cosmosDbService;
    }

    public List<Vehicle> Vehicles { get; set; } = [];

    public async Task OnGetAsync()
    {
        Vehicles = await _cosmosDbService.GetVehiclesAsync();
    }
}
