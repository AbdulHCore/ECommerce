using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;

namespace Common.Logging
{
    public static class Logging
    {
        public static Action<HostBuilderContext, LoggerConfiguration> ConfigureLogger => (context, loggerConfig) =>
        {
            var env = context.HostingEnvironment;
            loggerConfig.MinimumLevel.Information()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ApplicationName", env.ApplicationName)
            .Enrich.WithProperty("EnvironmentName", env.EnvironmentName)
            .Enrich.WithExceptionDetails()
            .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", Serilog.Events.LogEventLevel.Warning)
            .WriteTo.Console();

            if (env.IsDevelopment())
            {
                loggerConfig.MinimumLevel.Override("Catalog", Serilog.Events.LogEventLevel.Debug);
                loggerConfig.MinimumLevel.Override("Basket", Serilog.Events.LogEventLevel.Debug);
                loggerConfig.MinimumLevel.Override("Discount", Serilog.Events.LogEventLevel.Debug);
                loggerConfig.MinimumLevel.Override("Ordering", Serilog.Events.LogEventLevel.Debug);
            }

            //Elastic Search - Add loging onto Elastic Search
            var elasticUrl = context.Configuration.GetValue<string>("ElasticConfiguration:Uri");
            if (!string.IsNullOrEmpty(elasticUrl))
            {
                loggerConfig.WriteTo.Elasticsearch(
                    new ElasticsearchSinkOptions(new Uri(elasticUrl))
                        { 
                            AutoRegisterTemplate = true,
                            AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv8,
                            IndexFormat = "ecommerce-logs-{0:yyyy.MM.dd}",
                            MinimumLogEventLevel = Serilog.Events.LogEventLevel.Debug
                        });
            }
        };
    }
}
