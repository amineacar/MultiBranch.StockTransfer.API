using Microsoft.EntityFrameworkCore;
using MultiBranch.StockTransfer.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MultiBranch.StockTransfer.Application.Interfaces;
using MultiBranch.StockTransfer.Application.Services;

namespace MultiBranch.StockTransfer.Infrastructure;

public static class DependencyInjection
{
     public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(
                    typeof(ApplicationDbContext).Assembly.FullName)
                    ));
        services.AddScoped<IApplicationDbContext>(provider =>
        provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IShelfService, ShelfService>();
        services.AddScoped<IShelfStockService, ShelfStockService>();
        services.AddScoped<IStoreService, StoreService>();
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<ITransferService, TransferService>();
        return services;
    }
}
