using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderIngestionFunction.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        // Register custom services for dependency injection.
        // Use Singleton for stateless services and Scoped/Transient for services with state or dependencies like ILogger.
        services.AddSingleton<IOrderValidator, OrderValidator>();
        services.AddSingleton<IProductService, ProductService>();
        services.AddSingleton<ICustomerService, CustomerService>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IInventoryNotifier, InventoryNotifier>();
    })
    .Build();

host.Run();