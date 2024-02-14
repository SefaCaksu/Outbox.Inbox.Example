using System.Text.Json;
using MassTransit;
using Shared.Events;
using Stock.API.Models.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        readonly StockDbContext _stockDbContext;

        public OrderCreatedEventConsumer(StockDbContext stockDbContext)
        {
            _stockDbContext = stockDbContext;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            bool result = await _stockDbContext.OrderInboxes.AnyAsync(c => c.IdemPotentToken == context.Message.IdempotentToken);
            if (!result)
            {

                await _stockDbContext.OrderInboxes.AddAsync(new()
                {
                    Processed = false,
                    Payload = JsonSerializer.Serialize(context.Message)
                });

                await _stockDbContext.SaveChangesAsync();
            }
        }
    }
}

