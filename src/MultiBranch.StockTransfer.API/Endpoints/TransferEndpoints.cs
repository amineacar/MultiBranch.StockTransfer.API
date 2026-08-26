using FluentValidation;
using MultiBranch.StockTransfer.Application.DTOs.Transfer;
using MultiBranch.StockTransfer.Application.Interfaces;

namespace MultiBranch.StockTransfer.API.Endpoints;

public static class TransferEndpoints
{
    public static void MapTransferEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/transfers")
            .WithTags("Transfers");
        // GET ALL
        group.MapGet("/", async (ITransferService service) =>
        {
            var transfers = await service.GetAllAsync();
            return Results.Ok(transfers);
        });

        // GET BY ID
        group.MapGet("/{id:guid}", async (
            Guid id,
            ITransferService service) =>
        {
            var transfer = await service.GetByIdAsync(id);

            return transfer is null
                ? Results.NotFound(new
                {
                    message = "Transfer not found."
                })
                : Results.Ok(transfer);
        });

        // CREATE 
        group.MapPost("/", async (
            CreateTransferDto dto,
            ITransferService service) =>
        { try
            {
                var transfer = await service.CreateAsync(dto);

                return Results.Created(
                    $"/api/transfers/{transfer.Id}",
                    transfer);
            }
            catch (ValidationException ex)
            {
                return Results.BadRequest(new
                {
                    message = "Validation failed.",
                    errors = ex.Errors.Select(e => new
                    {
                        field = e.PropertyName,
                        message = e.ErrorMessage
                    })
                });
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new
                {
                    message = ex.Message
                });
            }
        });

        // COMPLETE 
        group.MapPost("/{id:guid}/complete", async (
            Guid id,
            Guid employeeId,
            ITransferService service) =>
        {
            try
            {
                var transfer = await service.CompleteAsync(
                    id,
                    employeeId);

                return transfer is null
                    ? Results.NotFound(new
                    {
                        message = "Transfer not found or is no longer in transit."
                    })
                    : Results.Ok(transfer);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new
                {
                    message = ex.Message
                });
            }
        });

        // CANCEL 
        group.MapPost("/{id:guid}/cancel", async (
            Guid id,
            Guid employeeId,
            ITransferService service) =>
        {
            try {
                var transfer = await service.CancelAsync(
                    id,
                    employeeId);

                return transfer is null
                    ? Results.NotFound(new
                    {
                        message = "Transfer not found or is no longer in transit."
                    })
                    : Results.Ok(transfer);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new
                {
                    message = ex.Message
                });
            }
        });
    }
}

