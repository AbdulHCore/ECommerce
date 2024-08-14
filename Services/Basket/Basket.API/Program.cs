using Asp.Versioning;
using Basket.Application.GrpcService;
using Basket.Application.Queries;
using Basket.Core.Repositories;
using Basket.Infrastructure.Repositories;
using Common.Logging;
using Discount.Grpc.Protos;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

//Serilog Configuration, via Common.Logging.
builder.Host.UseSerilog(Logging.ConfigureLogger);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
}) 
    //Below bit to use add different version of the controller: Ex: BasketControllerV2
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

//Add APIVersion on Swagger Doc.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => 
{ 
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Basket.API", Version = "v1" });
    c.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Basket.API", Version = "v2" });

    //include XML Comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);

    //Configure Swagger to use Versioning
    c.DocInclusionPredicate((version, apiDesription) => 
    {
        if (!apiDesription.TryGetMethodInfo(out var method))
            return false;
        var versions = method.DeclaringType?
                        .GetCustomAttributes(true)
                        .OfType<ApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions);
        return versions?.Any(v => $"v{v.ToString()}" == version) ?? false;
        
    });
});

//Register AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

//Register MediatR - to achieve CQRS Pattern
var assemblies = new Assembly[]
{
    Assembly.GetExecutingAssembly(),
    typeof(GetBasketByUserNameQuery).Assembly
};

builder.Services.AddMediatR (cfg=> cfg.RegisterServicesFromAssemblies(assemblies));

//Register Cache - Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetValue<string>("CacheSettings:ConnectionString");
});

//Application Services
builder.Services.AddScoped<IBasektRepository, BasketRepository>();
builder.Services.AddScoped<DiscountGrpcService>();
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>
    (cfg => cfg.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]));

//Wiring up Rabbit MQ on API via MassTransit Library
builder.Services.AddMassTransit(config =>
    {
        config.UsingRabbitMq((ctx, cfg) =>
            cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]));
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
        options.Audience = "Basket";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI( c=>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket.API v1");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "Basket.API v2");
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
