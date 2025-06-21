using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrderIngestionFunction.Models;
using OrderIngestionFunction.Services;
using System.Net;

namespace OrderIngestionFunction
{
    public class OrderIngestionHttpTrigger
    {
        private readonly ILogger<OrderIngestionHttpTrigger> _logger;
        private readonly IOrderValidator _orderValidator;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly IOrderRepository _orderRepository;
        private readonly IInventoryNotifier _inventoryNotifier;

        public OrderIngestionHttpTrigger(
            ILogger<OrderIngestionHttpTrigger> logger,
            IOrderValidator orderValidator,
            ICustomerService customerService,
            IProductService productService,
            IOrderRepository orderRepository,
            IInventoryNotifier inventoryNotifier)
        {
            _logger = logger;
            _orderValidator = orderValidator;
            _customerService = customerService;
            _productService = productService;
            _orderRepository = orderRepository;
            _inventoryNotifier = inventoryNotifier;
        }

        [Function("IngestOrder")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            HttpResponseData response;

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Order order = JsonConvert.DeserializeObject<Order>(requestBody);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

                if (order == null)
                {
                    response = req.CreateResponse(HttpStatusCode.BadRequest);
                    await response.WriteStringAsync("Invalid order data provided.");
                    _logger.LogError("Invalid order data received: Order is null.");
                    return response;
                }

                // --- 1. Validation ---
                var validationErrors = _orderValidator.Validate(order);
                if (validationErrors.Any())
                {
                    response = req.CreateResponse(HttpStatusCode.BadRequest);
                    await response.WriteStringAsync($"Order validation failed: {string.Join(", ", validationErrors)}");
                    _logger.LogWarning("Order validation failed for OrderId: {OrderId}. Errors: {Errors}", order.OrderId, string.Join(", ", validationErrors));
                    return response;
                }

                _logger.LogInformation("Order {OrderId} validated successfully.", order.OrderId);

                // --- 2. Consultar adicional: Precio, Disponibilidad, SKU y Cliente ---
                var customerInfo = await _customerService.GetCustomerInfo(order.CustomerId);
                if (customerInfo == null)
                {
                    response = req.CreateResponse(HttpStatusCode.BadRequest);
                    await response.WriteStringAsync($"Customer {order.CustomerId} not found.");
                    _logger.LogError("Customer {CustomerId} not found for OrderId: {OrderId}", order.CustomerId, order.OrderId);
                    return response;
                }
                _logger.LogInformation("Customer {CustomerId} information retrieved. Membership: {MembershipLevel}", customerInfo.CustomerId, customerInfo.MembershipLevel);

                foreach (var item in order.Items)
                {
                    var productInfo = await _productService.GetProductInfo(item.Sku);
                    if (productInfo == null || !productInfo.IsAvailable)
                    {
                        response = req.CreateResponse(HttpStatusCode.BadRequest);
                        await response.WriteStringAsync($"Product SKU {item.Sku} not found or not available.");
                        _logger.LogError("Product SKU {Sku} not found or not available for OrderId: {OrderId}", item.Sku, order.OrderId);
                        return response;
                    }
                    _logger.LogInformation("Product SKU {Sku} info retrieved. Price: {Price}, Available: {IsAvailable}", item.Sku, productInfo.Price, productInfo.IsAvailable);
                }

                // --- 3. Simular guardado en base de datos ---
                bool dbSaveSuccess = await _orderRepository.SaveOrderAsync(order);
                if (!dbSaveSuccess)
                {
                    response = req.CreateResponse(HttpStatusCode.InternalServerError);
                    await response.WriteStringAsync("Failed to save order to database.");
                    _logger.LogError("Failed to save OrderId: {OrderId} to database.", order.OrderId);
                    return response;
                }

                // --- 4. Notificar al inventario para actualizar stock ---
                bool inventoryUpdateSuccess = await _inventoryNotifier.NotifyForStockUpdateAsync(order);
                if (!inventoryUpdateSuccess)
                {
                    _logger.LogWarning("Failed to notify inventory for stock update for OrderId: {OrderId}. Order processed, but inventory might be out of sync.", order.OrderId);
                }

                response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteStringAsync($"Order {order.OrderId} ingested successfully!");
            }
            catch (JsonSerializationException ex)
            {
                response = req.CreateResponse(HttpStatusCode.BadRequest);
                await response.WriteStringAsync($"Invalid JSON format: {ex.Message}");
                _logger.LogError(ex, "JSON deserialization error.");
            }
            catch (Exception ex)
            {
                response = req.CreateResponse(HttpStatusCode.InternalServerError);
                await response.WriteStringAsync($"An unexpected error occurred: {ex.Message}");
                _logger.LogError(ex, "An unhandled exception occurred during order ingestion.");
            }

            return response;
        }
    }
}