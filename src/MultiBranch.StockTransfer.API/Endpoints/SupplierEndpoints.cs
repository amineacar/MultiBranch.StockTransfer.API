using MultiBranch.StockTransfer.Application.DTOs.Supplier;
using MultiBranch.StockTransfer.Application.Interfaces;

namespace MultiBranch.StockTransfer.Endpoints;

public static class SupplierEndpoints
{
    public static void MapSupplierEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/suppliers")
            .WithTags("Suppliers");

        group.MapGet("/", async (ISupplierService service) =>
        {
            var suppliers = await service.GetAllAsync();

            return Results.Ok(suppliers);
        });

        group.MapGet("/{id:guid}", async (
            Guid id,
            ISupplierService service) =>
        {
            var supplier = await service.GetByIdAsync(id);

            return supplier is null
                ? Results.NotFound()
                : Results.Ok(supplier);
        });

        group.MapPost("/", async (
            CreateSupplierDto dto,
            ISupplierService service) =>
        {
            var supplier = await service.CreateAsync(dto);

            return Results.Created(
                $"/api/suppliers/{supplier.Id}",
                supplier);
        });

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateSupplierDto dto,
            ISupplierService service) =>
        {
            var supplier = await service.UpdateAsync(id, dto);

            return supplier is null
                ? Results.NotFound()
                : Results.Ok(supplier);
        });

        group.MapDelete("/{id:guid}", async (
            Guid id,
            ISupplierService service) =>
        {
            var deleted = await service.DeleteAsync(id);

            return deleted
                ? Results.NoContent()
                : Results.NotFound();
        });
    }
}