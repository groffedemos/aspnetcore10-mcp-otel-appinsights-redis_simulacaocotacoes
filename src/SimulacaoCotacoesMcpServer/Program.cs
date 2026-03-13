using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SimulacaoCotacoesMcpServer.Tools;
using SimulacaoCotacoesMcpServer.Tracing;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

using var connectionRedis = ConnectionMultiplexer.Connect(
    builder.Configuration.GetConnectionString("Redis")!);
builder.Services.AddSingleton(connectionRedis);

var resourceBuilder = ResourceBuilder.CreateDefault()
    .AddService(serviceName: OpenTelemetryExtensions.ServiceName,
        serviceVersion: OpenTelemetryExtensions.ServiceVersion);
builder.Services.AddOpenTelemetry()
    .WithTracing((traceBuilder) =>
    {
        traceBuilder
            .AddSource(OpenTelemetryExtensions.ServiceName)
            .SetResourceBuilder(resourceBuilder)
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRedisInstrumentation(connectionRedis)
            .AddAzureMonitorTraceExporter(options =>
            {
                options.ConnectionString = builder.Configuration.GetConnectionString("AppInsights");
            });
    });

builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithTools<CotacaoDolarTool>()
    .WithTools<CotacaoEuroTool>()
    .WithTools<CotacaoLibraTool>();

var app = builder.Build();

app.MapMcp();

app.Run();