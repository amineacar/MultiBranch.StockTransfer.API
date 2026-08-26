using MultiBranch.StockTransfer.Application.DTOs.Employee;
using MultiBranch.StockTransfer.Application.Interfaces;

namespace MultiBranch.StockTransfer.API.Endpoints;

public static class EmployeeEndpoints
{
    public static void MapEmployeeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/employees")
            .WithTags("Employees");

        group.MapGet("/", async (IEmployeeService service) =>
        {
            var employees = await service.GetAllAsync();

            return Results.Ok(employees);
        });

        group.MapGet("/{id:guid}", async (
            Guid id,
            IEmployeeService service) =>
        {
            var employee = await service.GetByIdAsync(id);

            return employee is null
                ? Results.NotFound()
                : Results.Ok(employee);
        });

        group.MapPost("/", async (
            CreateEmployeeDto dto,
            IEmployeeService service) =>
        {
            var employee = await service.CreateAsync(dto);

            return Results.Created(
                $"/api/employees/{employee.Id}",
                employee);
        });

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateEmployeeDto dto,
            IEmployeeService service) =>
        {
            var employee = await service.UpdateAsync(id, dto);

            return employee is null
                ? Results.NotFound()
                : Results.Ok(employee);
        });

        group.MapDelete("/{id:guid}", async (
            Guid id,
            IEmployeeService service) =>
        {
            var deleted = await service.DeleteAsync(id);

            return deleted
                ? Results.NoContent()
                : Results.NotFound();
        });
    }
}