using MultiBranch.StockTransfer.Application.DTOs.Store;
using MultiBranch.StockTransfer.Application.Interfaces;

namespace MultiBranch.StockTransfer.API.Endpoints;

public static class StoreEndpoints
{
    public static void MapStoreEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/stores")
            .WithTags("Stores");

        group.MapGet("/", async (IStoreService service) =>
        {
            var stores = await service.GetAllAsync();

            return Results.Ok(stores);
        });

        group.MapGet("/{id:guid}", async (
            Guid id,
            IStoreService service) =>
        {
            var store = await service.GetByIdAsync(id);

            return store is null
                ? Results.NotFound()
                : Results.Ok(store);
        });

        group.MapPost("/", async (
            CreateStoreDto dto,
            IStoreService service) =>
        {
            var store = await service.CreateAsync(dto);

            return Results.Created(
                $"/api/stores/{store.Id}",
                store);
        });

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateStoreDto dto,
            IStoreService service) =>
        {
            var store = await service.UpdateAsync(id, dto);

            return store is null
                ? Results.NotFound()
                : Results.Ok(store);
        });

        group.MapDelete("/{id:guid}", async (
            Guid id,
            IStoreService service) =>
        {
            var deleted = await service.DeleteAsync(id);

            return deleted
                ? Results.NoContent()
                : Results.NotFound();
        });
    }
}