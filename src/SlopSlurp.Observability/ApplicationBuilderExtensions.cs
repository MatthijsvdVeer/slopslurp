namespace SlopSlurp.Observability;

using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

public static class ApplicationBuilderExtensions
{
    public static IHostApplicationBuilder AddTelemetry(this IHostApplicationBuilder builder, string serviceName)
    {
        _ = builder.Logging.AddOpenTelemetry(options =>
        {
            options.IncludeScopes = true;
            options.IncludeFormattedMessage = true;
        });

        _ = builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource =>
            {
                _ = resource.AddService(serviceName: serviceName,
                    serviceNamespace: "slopslurp",
                    serviceVersion: "1.0",
                    autoGenerateServiceInstanceId: false,
                    serviceInstanceId: serviceName);
            })
            .WithMetrics(metrics =>
            {
                _ = metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();
            }).WithTracing(tracing =>
            {
                _ = tracing.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSource(ActivitySources.Validation.Name);
            });

        _ = builder.AddOpenTelemetryExporters();

        return builder;
    }

    private static IHostApplicationBuilder AddOpenTelemetryExporters(this IHostApplicationBuilder builder)
    {
        var exporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);
        if (exporter)
        {
            _ = builder.Services.AddOpenTelemetry()
                .UseOtlpExporter();
        }

        if (!string.IsNullOrEmpty(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
        {
            _ = builder.Services.AddOpenTelemetry()
                .UseAzureMonitor();
        }

        return builder;
    }
}
