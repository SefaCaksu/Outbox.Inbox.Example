using System;
using Quartz;
using MassTransit;
using Order.Outbox.Table.Publisher.Service.Entities;
using Shared.Events;
using System.Text.Json;

namespace Order.Outbox.Table.Publisher.Service.Jobs
{
    public class OrderOutboxPunblishJob : IJob
    {
        readonly IPublishEndpoint _publishEndPoint;

        public OrderOutboxPunblishJob(IPublishEndpoint publishEndPoint)
        {
            _publishEndPoint = publishEndPoint;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            if (OrderOutboxSingletonDatabase.DataReaderState)
            {
                List<OrderOutbox> orderOutboxes = (await OrderOutboxSingletonDatabase
                           .QueryAsync<OrderOutbox>("SELECT * FROM [OrderAPIDB-OUTBOX].dbo.OrderOutboxes WHERE ProcessedDate IS NULL ORDER BY OccuredOn ASC")).ToList();


                foreach (var orderOutBox in orderOutboxes)
                {
                    if(orderOutBox.Type == nameof(OrderCreatedEvent))
                    {
                        OrderCreatedEvent orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(orderOutBox.Payload);

                        if(orderCreatedEvent != null)
                        {
                            await _publishEndPoint.Publish(orderCreatedEvent);
                            OrderOutboxSingletonDatabase.ExecuteAsync($"UPDATE [OrderAPIDB-OUTBOX].dbo.OrderOutboxes SET PROCESSEDDATE = GETDATE() WHERE IdemPotentToken = '{orderOutBox.IdemPotentToken}'");
                        }
                    }
                }

                OrderOutboxSingletonDatabase.DataReaderReady();
                await Console.Out.WriteAsync("Order outbox checked");
            }
        }
    }
}

