using System.Net;
using System.Text.Json;
using LinuxVmCosmosdb.Web.Models;
using Microsoft.Azure.Cosmos;

namespace LinuxVmCosmosdb.Web.Services;

public class CosmosDbService
{
    private readonly CosmosClient _client;
    private const string DatabaseName = "logistics";
    private Container ShipmentsContainer => _client.GetContainer(DatabaseName, "shipments");
    private Container VehiclesContainer => _client.GetContainer(DatabaseName, "vehicles");
    private Container RoutesContainer => _client.GetContainer(DatabaseName, "routes");

    public CosmosDbService(CosmosClient client)
    {
        _client = client;
    }

    public async Task InitializeAsync()
    {
        var database = await _client.CreateDatabaseIfNotExistsAsync(DatabaseName);

        await database.Database.CreateContainerIfNotExistsAsync("shipments", "/id");
        await database.Database.CreateContainerIfNotExistsAsync("vehicles", "/id");
        await database.Database.CreateContainerIfNotExistsAsync("routes", "/id");

        await SeedDataIfEmptyAsync();
    }

    private async Task SeedDataIfEmptyAsync()
    {
        var shipmentCount = await GetCountAsync(ShipmentsContainer);
        if (shipmentCount == 0)
        {
            var seedPath = Path.Combine(AppContext.BaseDirectory, "SeedData");
            await SeedContainerAsync<Shipment>(ShipmentsContainer, Path.Combine(seedPath, "shipments.json"));
            await SeedContainerAsync<Vehicle>(VehiclesContainer, Path.Combine(seedPath, "vehicles.json"));
            await SeedContainerAsync<DeliveryRoute>(RoutesContainer, Path.Combine(seedPath, "routes.json"));
        }
    }

    private static async Task<int> GetCountAsync(Container container)
    {
        try
        {
            var query = container.GetItemQueryIterator<int>("SELECT VALUE COUNT(1) FROM c");
            var response = await query.ReadNextAsync();
            return response.FirstOrDefault();
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return 0;
        }
    }

    private static async Task SeedContainerAsync<T>(Container container, string jsonPath)
    {
        if (!File.Exists(jsonPath)) return;

        var json = await File.ReadAllTextAsync(jsonPath);
        var items = JsonSerializer.Deserialize<List<T>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (items is null) return;

        foreach (var item in items)
        {
            await container.UpsertItemAsync(item);
        }
    }

    // Shipments
    public async Task<List<Shipment>> GetShipmentsAsync()
    {
        var query = ShipmentsContainer.GetItemQueryIterator<Shipment>("SELECT * FROM c");
        var results = new List<Shipment>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }
        return results;
    }

    public async Task<Shipment?> GetShipmentAsync(string id)
    {
        try
        {
            var response = await ShipmentsContainer.ReadItemAsync<Shipment>(id, new PartitionKey(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task UpsertShipmentAsync(Shipment shipment)
    {
        await ShipmentsContainer.UpsertItemAsync(shipment, new PartitionKey(shipment.Id));
    }

    public async Task DeleteShipmentAsync(string id)
    {
        await ShipmentsContainer.DeleteItemAsync<Shipment>(id, new PartitionKey(id));
    }

    // Vehicles
    public async Task<List<Vehicle>> GetVehiclesAsync()
    {
        var query = VehiclesContainer.GetItemQueryIterator<Vehicle>("SELECT * FROM c");
        var results = new List<Vehicle>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }
        return results;
    }

    public async Task<Vehicle?> GetVehicleAsync(string id)
    {
        try
        {
            var response = await VehiclesContainer.ReadItemAsync<Vehicle>(id, new PartitionKey(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task UpsertVehicleAsync(Vehicle vehicle)
    {
        await VehiclesContainer.UpsertItemAsync(vehicle, new PartitionKey(vehicle.Id));
    }

    // Routes
    public async Task<List<DeliveryRoute>> GetRoutesAsync()
    {
        var query = RoutesContainer.GetItemQueryIterator<DeliveryRoute>("SELECT * FROM c");
        var results = new List<DeliveryRoute>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }
        return results;
    }

    public async Task<DeliveryRoute?> GetRouteAsync(string id)
    {
        try
        {
            var response = await RoutesContainer.ReadItemAsync<DeliveryRoute>(id, new PartitionKey(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task UpsertRouteAsync(DeliveryRoute route)
    {
        await RoutesContainer.UpsertItemAsync(route, new PartitionKey(route.Id));
    }
}
