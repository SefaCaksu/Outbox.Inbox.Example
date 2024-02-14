using System.Text.Json;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Models;
using Order.API.Models.Contexts;
using Order.API.ViewModels;
using Shared;
using Shared.Events;
using Shared.Messages;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<OrderApiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer")));

builder.Services.AddMassTransit(configurator =>
{
    configurator.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(builder.Configuration["RabbitMQ"]);
    });
});

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/create-order", async (CreateOrderVM model, OrderApiContext context, ISendEndpointProvider sendEndpointProvider) =>
{
    Order.API.Models.Order order = new Order.API.Models.Order()
    {
        BuyerId = model.BuyerId,
        OrderItems = model.OrderItems.Select(c => new OrderItem
        {
            Count = c.Count,
            Price = c.Price,
            ProductId = c.ProductId
        }).ToList(),
        TotalPrice = model.OrderItems.Sum(c => c.Price * c.Count)
    };

    await context.Orders.AddAsync(order);

    Guid idemptentToken = Guid.NewGuid();
    OrderCreatedEvent orderCreatedEvent = new()
    {
        BuyerId = order.BuyerId,
        OrderId = order.Id,
        TotalPrice = order.TotalPrice,
        OrderItems = order.OrderItems.Select(c => new OrderItemMessage
        {
            Count = c.Count,
            Price = c.Price,
            ProductId = c.ProductId
        }).ToList(),
        IdempotentToken = idemptentToken
    };

    //var sendEndPoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.Stock_OrderCreatedEventQueue}"));
    //await sendEndPoint.Send(orderCreatedEvent);

    OrderOutbox orderOutbox = new()
    {
        IdemPotentToken = idemptentToken,
        OccuredOn = DateTime.Now,
        ProcessedDate = null,
        Payload = JsonSerializer.Serialize(orderCreatedEvent),
        Type = orderCreatedEvent.GetType().Name
    };

    await context.OrderOutboxes.AddAsync(orderOutbox);
    await context.SaveChangesAsync();

});

app.Run();

