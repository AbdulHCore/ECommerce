using Microsoft.AspNetCore.Authentication.BearerToken;
using Catalog.Core.Repositories;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Repositories;
using Catalog.Application.Handlers;
using System.Reflection;
using Serilog;
using Common.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

//Serilog Configuration, via Common.Logging.
builder.Host.UseSerilog(Logging.ConfigureLogger);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c=> { c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Catalog.API", Version = "v1" }); });

//Custom Middlewares registration
//AutoMapper Injetion
builder.Services.AddAutoMapper(typeof(Program).Assembly);

//Mediator Injection
var assemblies = new Assembly[]
{
    Assembly.GetExecutingAssembly(),
    typeof(GetAllBrandsHandler).Assembly
};
builder.Services.AddMediatR(cfg=> cfg.RegisterServicesFromAssemblies(assemblies));

//Inject all Customer repository & contexts
builder.Services.AddScoped<ICatalogContext, CatalogContext>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IBrandRepository, ProductRepository>();
builder.Services.AddScoped<IProductTypeRepository, ProductRepository>();

//Identity Server Changes
//Adding Auth policy
//var userPolicy = new AuthorizationPolicyBuilder()
//    .RequireAuthenticatedUser()
//    .Build();

//Marking all the controllers should use the Auth Policy
//builder.Services.AddControllers(config =>
//{
//    config.Filters.Add(new AuthorizeFilter(userPolicy));
//});

//To contact Identity Server to provide token
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        //Use the below commented Authority when NON-SSL mode authorisation used.
//        //options.Authority = "https://localhost:8009";

//        //Use the below when Authority should be SSL mode.  As this port and certificate exposed to docker
//        options.Authority = "https://id-local.eshopping.com:44344";
//        options.Audience = "Catalog";
//    });

//Enable specific scope level access instead of default full access
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("CanRead", policy => policy.RequireClaim("scope", "catalogapi.read"));
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseAuthentication();
//app.UseAuthorization();

app.MapControllers();

app.Run();
