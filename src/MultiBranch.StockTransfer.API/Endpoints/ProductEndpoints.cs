using MultiBranch.StockTransfer.Application.DTOs.Product;
using MultiBranch.StockTransfer.Application.Interfaces;

namespace MultiBranch.StockTransfer.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products")
            .WithTags("Products");

        group.MapGet("/", async (IProductService service) =>
        {
            var products = await service.GetAllAsync();

            return Results.Ok(products);
        });

        group.MapGet("/{id:guid}", async (
            Guid id,
            IProductService service) =>
        {
            var product = await service.GetByIdAsync(id);

            return product is null
                ? Results.NotFound()
                : Results.Ok(product);
        });

        group.MapPost("/", async (
            CreateProductDto dto,
            IProductService service) =>
        {
            var product = await service.CreateAsync(dto);

            return Results.Created(
                $"/api/products/{product.Id}",
                product);
        });

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateProductDto dto,
            IProductService service) =>
        {
            var product = await service.UpdateAsync(id, dto);

            return product is null
                ? Results.NotFound()
                : Results.Ok(product);
        });

        group.MapDelete("/{id:guid}", async (
            Guid id,
            IProductService service) =>
        {
            var deleted = await service.DeleteAsync(id);

            return deleted
                ? Results.NoContent()
                : Results.NotFound();
        });
    }
}