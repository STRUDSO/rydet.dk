#pragma warning disable MA0048
var builder = DistributedApplication.CreateBuilder(args);
#pragma warning restore MA0048

builder.AddDockerComposeEnvironment("compose")
    .WithDashboard(dashboard =>
    {
        dashboard.WithHostPort(8080)
            .WithForwardedHeaders(enabled: true);
    })
    ;

var postgres = builder
    .AddPostgres("postgres")
    .WithPgAdmin(x => x.WithExplicitStart())
    .PublishAsDockerComposeService((resource, service) => { })
    .WithOtlpExporter()
    ;

var postgresdb = postgres.AddDatabase("postgresdb");

var flagd = builder.AddFlagd("flagd")
    .WithBindFileSync("./flags/")
    // .WithArgs(
    //     "--metrics-exporter", "otel"
    // )
    // .WithArgs(x =>
#pragma warning disable S125
    // {
#pragma warning restore S125
    //     var s = builder.Configuration["ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL"]!;
    //     x.Args.Add("--otel-collector-uri");
    //     var uri = new Uri(s);
    //     x.Args.Add($"host.docker.internal:{uri.Port}");
    // })
    .WithOtlpExporter()
    .WithLogLevel(Microsoft.Extensions.Logging.LogLevel.Debug)
    ;

builder
    .AddProject<Projects.Rydet_ApiService>("apiservice")
    .WaitFor(postgresdb)
    .WithReference(postgresdb)
    .WithReference(flagd)
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .PublishAsDockerComposeService((resource, service) => { })
    ;

await builder.Build().RunAsync();
