using Asp.Versioning;
using Common.Logging;
using EventBus.Messages.Common;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Ordering.API.EventBusConsumer;
using Ordering.API.Extensions;
using Ordering.Application.Extensions;
using Ordering.Infrastructure.Data;
using Ordering.Infrastructure.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//Serilog Configuration, via Common.Logging.
builder.Host.UseSerilog(Logging.ConfigureLogger);

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
builder.Services.AddScoped<BasketOrderingConsumerV2>();

builder.Services.AddMassTransit(config =>
    {
        config.AddConsumer<BasketOrderingConsumer>();
        config.AddConsumer<BasketOrderingConsumerV2>();
        config.UsingRabbitMq((ctx, cfg) =>
        {
            cfg.Host(builder.Configuration["EventBussSettings:HostAddress"]);
            //Provide QueueName setting
            cfg.ReceiveEndpoint(EventBusConstant.BasketCheckoutQueue, c =>
            {
                c.ConfigureConsumer<BasketOrderingConsumer>(ctx);
            });

            //V2 Version
            cfg.ReceiveEndpoint(EventBusConstant.BasketCheckoutQueueV2, c => //If there is a problem, then create New Queue for V2, and use the same.
            {
                c.ConfigureConsumer<BasketOrderingConsumerV2>(ctx);
            });
        });
    });
builder.Services.AddMassTransitHostedService();

//Identity Server Changes
//Adding Auth policy
var userPolicy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();

//Marking all the controllers should use the Auth Policy
builder.Services.AddControllers(config =>
{
    config.Filters.Add(new AuthorizeFilter(userPolicy));
});

//To contact Identity Server to provide token
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:8009";
        options.Audience = "Ordering";
    });

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

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
