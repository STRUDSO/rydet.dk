#pragma warning disable MA0048
var builder = DistributedApplication.CreateBuilder(args);
#pragma warning restore MA0048

builder
    .AddProject<Projects.Rydet_ApiService>("apiservice")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health");

await builder.Build().RunAsync();
