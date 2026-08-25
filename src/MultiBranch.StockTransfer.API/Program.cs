using MultiBranch.StockTransfer.Infrastructure;
using FluentValidation;
using MultiBranch.StockTransfer.Application.DTOs.Transfer;
using MultiBranch.StockTransfer.Application.Validators.Transfer;
using MultiBranch.StockTransfer.Endpoints;
using MultiBranch.StockTransfer.API.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IValidator<CreateTransferDto>, CreateTransferValidator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapCategoryEndpoints();
app.MapProductEndpoints();
app.MapSupplierEndpoints ();
app.MapStoreEndpoints();
app.MapEmployeeEndpoints();
app.MapShelfEndpoints();
app.MapShelfStockEndpoints();
app.MapTransferEndpoints();
app.Run();
