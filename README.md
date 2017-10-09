# Astral

Included libraries:
- Astral.Core - base interfaces, extensions and Result and Option monad for .Net
- Astral.Markup - attribute based .net service markup
- Astral.Schema - service schema builder and AST
- Astral - utility library for service integration
- Astral.RabbitLink - service bus realisation for RabbitLink (RabbitMq based)

## Usage.
### Decalare service.
```csharp
[Owner("main")]
[Service("orders")]
public interface IOrderService
{
    [Endpoint("order.changed")] 
    EventHandler<Order> OrderChanged { get; }
    
    [Endpoint("order.create")]
    Func<Order, int> CreateOrder { get; }
    
    [Endpoint("order.update")]
    Action<Order> UpdateOrder { get; }
}

[Contract("order")]
public class Order
{
    public int Number { get; set; }
    public string Customer { get; set; }
    public List<OrderLine> Lines { get; set; }
}

public class OrderLine
{
    public string GoodCode { get; set; }
    public double Quantity { get; set; }
    public double Price { get; set; }
}
```
#### RabbitMq behavior
- OrderChanged - Exchange name "main.orders", exchange type - direct, routing key "order.changed", message type - "order", message content type - "text/json;charset-utf-8"
- CreateOrder - Request exchange name "main.orders", exchange type - direct, routing key "order.create", message type - "order",
message content type - "text/json;charset-utf-8", response exchange name "main.orders", request queue name - "main.orders.order.create" 
- UpdateOrder - Request exchange name "main.orders", exchange type - direct, routing key "order.update", message type - "order",
message content type - "text/json;charset-utf-8", response exchange name "main.orders", request queue name - "main.orders.order.update" 

### RabbitMq customization:
Attribute based:
- ExchangeAttribute - specify exchange parameters
- ResponseExchange - specify response exchange parameters
- RpcQueueAttribute - specify request queue parameters
- RoutingKeyAttribute - specify routing key

### Server usage.
```csharp
using (var link =
            new ServiceLinkBuilder()
                .HolderName("testserver")
                .Uri("amqp://localhost")
                .AutoStart(true)
                .ConnectionName("Test process")
                .Build())
{
    link.Service<IOrderService>().Call(p => p.CreateOrder)
        .Process((order, cancellation) => Task.FromResult(25));  
    link.Service<IOrderService>().Event(p => p.OrderChanged)
        .PublishAsync(new Order { Numder = 25 });
    Console.ReadKey();    
}
```
### Client usage.
```csharp
using (var link =
            new ServiceLinkBuilder()
                .HolderName("testclient")
                .Uri("amqp://localhost")
                .AutoStart(true)
                .ConnectionName("Test process")
                .Build())
{
    var id = await link.Service<IOrderService>().Call(p => p.CreateOrder)
        .Call(new Order { Numder = 25 });  
    link.Service<IOrderService>().Event(p => p.OrderChanged)
        .Listen((order, cancellation) => Console.WriteLine($"Order {order.Numder} has been changed");
    
    Console.ReadKey();    
}
```




