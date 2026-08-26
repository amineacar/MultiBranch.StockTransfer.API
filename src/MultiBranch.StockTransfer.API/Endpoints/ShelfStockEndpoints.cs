using MultiBranch.StockTransfer.Application.DTOs.ShelfStock;
using MultiBranch.StockTransfer.Application.Interfaces;

namespace MultiBranch.StockTransfer.API.Endpoints;

public static class ShelfStockEndpoints
{
    public static void MapShelfStockEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/shelfstocks")
            .WithTags("Shelf Stocks");

        group.MapGet("/", async (IShelfStockService service) =>
        {
            var shelfStocks = await service.GetAllAsync();

            return Results.Ok(shelfStocks);
        });

        group.MapGet("/{id:guid}", async (
            Guid id,
            IShelfStockService service) =>
        {
            var shelfStock = await service.GetByIdAsync(id);

            return shelfStock is null
                ? Results.NotFound()
                : Results.Ok(shelfStock);
        });

        group.MapPost("/", async (
            CreateShelfStockDto dto,
            IShelfStockService service) =>
        {
            var shelfStock = await service.CreateAsync(dto);

            return Results.Created(
                $"/api/shelfstocks/{shelfStock.Id}",
                shelfStock);
        });

        group.MapDelete("/{id:guid}", async (
            Guid id,
            IShelfStockService service) =>
        {
            var deleted = await service.DeleteAsync(id);

            return deleted
                ? Results.NoContent()
                : Results.NotFound();
        });
         
 
   
        group.MapPost("/stock-in", async (
           StockMovementOperationDto dto,
           IShelfStockService service) =>
        {
        try{
           var result = await service.StockInAsync(dto);

           return Results.Ok(result);
        }
        catch (InvalidOperationException ex)
        {
           return Results.BadRequest(new
        {
            message = ex.Message
        });
        }});

        
        group.MapPost("/stock-out", async (
           StockMovementOperationDto dto,
           IShelfStockService service) =>
        {
        try{
          var result = await service.StockOutAsync(dto);
          return Results.Ok(result);
        }
        catch (InvalidOperationException ex)
        {
          return Results.BadRequest(new
        {
            message = ex.Message
        });
    }});

        group.MapPost("/relocate", async (
           Guid sourceShelfId,
           Guid targetShelfId,
           Guid productId,
           Guid employeeId,
           int quantity,
           string reason,
           IShelfStockService service) =>
        {
        try{
        await service.RelocateAsync(
            sourceShelfId,
            targetShelfId,
            productId,
            employeeId,
            quantity,
            reason);

        return Results.Ok(new
        {
            message = "Stock relocated successfully."
        });
        }
        catch (InvalidOperationException ex)
        {
        return Results.BadRequest(new
        {
            message = ex.Message
        });
    }});

    }
    
}