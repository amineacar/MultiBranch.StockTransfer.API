using MultiBranch.StockTransfer.Application.DTOs.Shelf;
using MultiBranch.StockTransfer.Application.Interfaces;

namespace MultiBranch.StockTransfer.API.Endpoints;

public static class ShelfEndpoints
{
    public static void MapShelfEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/shelves")
            .WithTags("Shelves");

        group.MapGet("/", async (IShelfService service) =>
        {
            var shelves = await service.GetAllAsync();

            return Results.Ok(shelves);
        });

        group.MapGet("/{id:guid}", async (
            Guid id,
            IShelfService service) =>
        {
            var shelf = await service.GetByIdAsync(id);

            return shelf is null
                ? Results.NotFound()
                : Results.Ok(shelf);
        });

        group.MapPost("/", async (
            CreateShelfDto dto,
            IShelfService service) =>
        {
            var shelf = await service.CreateAsync(dto);

            return Results.Created(
                $"/api/shelves/{shelf.Id}",
                shelf);
        });

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateShelfDto dto,
            IShelfService service) =>
        {
            var shelf = await service.UpdateAsync(id, dto);

            return shelf is null
                ? Results.NotFound()
                : Results.Ok(shelf);
        });

        group.MapDelete("/{id:guid}", async (
            Guid id,
            IShelfService service) =>
        {
            var deleted = await service.DeleteAsync(id);

            return deleted
                ? Results.NoContent()
                : Results.NotFound();
        });
    }
}