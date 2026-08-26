using MultiBranch.StockTransfer.Application.DTOs.Category;
using MultiBranch.StockTransfer.Application.Interfaces;

namespace MultiBranch.StockTransfer.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/categories")
            .WithTags("Categories");

        group.MapGet("/", async (ICategoryService service) =>
        {
            var categories = await service.GetAllAsync();

            return Results.Ok(categories);
        });

        group.MapGet("/{id:guid}", async (Guid id, ICategoryService service) =>
        {
            var category = await service.GetByIdAsync(id);

            return category is null
                ? Results.NotFound()
                : Results.Ok(category);
        });

        group.MapPost("/", async (
            CreateCategoryDto dto,
            ICategoryService service) =>
        {
            var category = await service.CreateAsync(dto);

            return Results.Created($"/api/categories/{category.Id}", category);
        });

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateCategoryDto dto,
            ICategoryService service) =>
        {
            var category = await service.UpdateAsync(id, dto);

            return category is null
                ? Results.NotFound()
                : Results.Ok(category);
        });

        group.MapDelete("/{id:guid}", async (
            Guid id,
            ICategoryService service) =>
        {
            var deleted = await service.DeleteAsync(id);

            return deleted
                ? Results.NoContent()
                : Results.NotFound();
        });
    }
}