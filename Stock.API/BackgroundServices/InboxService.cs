using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Shared.Events;
using Stock.API.Models.Contexts;
using Stock.API.Models.Entities;

namespace Stock.API.BackgroundServices
{
    public class InboxService : BackgroundService
    {
        readonly IServiceScopeFactory _scopeFactory;
        public InboxService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                await CallInbox(stoppingToken);
            }
        }

        private async Task CallInbox(CancellationToken cancellationToken)
        {
            using (IServiceScope scope = _scopeFactory.CreateScope())
            {
                StockDbContext stockDbContext = scope.ServiceProvider.GetRequiredService<StockDbContext>();

                List<OrderInbox> orderInboxes = await stockDbContext.OrderInboxes.Where(c => !c.Processed).ToListAsync();

                foreach (var orderInbox in orderInboxes)
                {
                    var orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(orderInbox.Payload);

                    //Gerekli işlemlerin yapıldığını varsay.
                    await Console.Out.WriteLineAsync(orderCreatedEvent?.OrderId.ToString());

                    orderInbox.Processed = true;
                    await stockDbContext.SaveChangesAsync();
                }

            }

            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        }
    }
}

