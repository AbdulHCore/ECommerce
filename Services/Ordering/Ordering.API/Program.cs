using Asp.Versioning;
using EventBus.Messages.Common;
using MassTransit;
using Ordering.API.EventBusConsumer;
using Ordering.API.Extensions;
using Ordering.Application.Extensions;
using Ordering.Infrastructure.Data;
using Ordering.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//Add API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c=> { c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Ordering.API", Version = "v1" }); });

//Application Services
builder.Services.AddApplicationServices();
builder.Services.AddInfraServices(builder.Configuration);

//RabbitMQ & MassTransit wireup for Message Consumer
builder.Services.AddScoped<BasketOrderingConsumer>();
builder.Services.AddMassTransit(config =>
    {
        config.AddConsumer<BasketOrderingConsumer>();
        config.UsingRabbitMq((ctx, cfg) =>
        {
            cfg.Host(builder.Configuration["EventBussSettings:HostAddress"]);
            //Provide QueueName setting
            cfg.ReceiveEndpoint(EventBusConstant.BasketCheckoutQueue, c =>
            {
                c.ConfigureConsumer<BasketOrderingConsumer>(ctx);
            });            
        });
    });
builder.Services.AddMassTransitHostedService();

var app = builder.Build();

//Apply DB Migration
app.MigrateDatabase<OrderContext>((context, services) =>
{
    var logger = services.GetService<ILogger<OrderContextSeed>>();
    OrderContextSeed.SeedAsync(context, logger);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
