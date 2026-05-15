using LinuxVmCosmosdb.Web.Models;
using LinuxVmCosmosdb.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LinuxVmCosmosdb.Web.Pages.Routes;

public class IndexModel : PageModel
{
    private readonly CosmosDbService _cosmosDbService;

    public IndexModel(CosmosDbService cosmosDbService)
    {
        _cosmosDbService = cosmosDbService;
    }

    public List<DeliveryRoute> Routes { get; set; } = [];

    public async Task OnGetAsync()
    {
        Routes = await _cosmosDbService.GetRoutesAsync();
    }
}
