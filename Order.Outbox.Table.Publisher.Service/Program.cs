
using MassTransit;
using Order.Outbox.Table.Publisher.Service.Jobs;
using Quartz;

var builder = Host.CreateApplicationBuilder(args);


builder.Services.AddMassTransit(configurator =>
{
    configurator.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(builder.Configuration["RabbitMQ"]);
    });
});


builder.Services.AddQuartz(c =>
{
    c.UseMicrosoftDependencyInjectionJobFactory();

    JobKey key = new JobKey("OrderOutboxPublishJob");
    c.AddJob<OrderOutboxPunblishJob>(options => options.WithIdentity(key));

    TriggerKey triggerKey = new TriggerKey("OrderOutboxPublishTrigger");
    c.AddTrigger(options => options.ForJob(key)
                           .WithIdentity(triggerKey)
                           .WithCronSchedule("0/5 * * * * ?")
    );
});

builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

var host = builder.Build();
host.Run();


