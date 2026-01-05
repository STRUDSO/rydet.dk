#pragma warning disable MA0048
var builder = DistributedApplication.CreateBuilder(args);
#pragma warning restore MA0048

var postgres = builder
    .AddPostgres("postgres")
    .WithPgAdmin(x => x.WithExplicitStart());

var postgresdb = postgres.AddDatabase("postgresdb");


builder
    .AddProject<Projects.Rydet_ApiService>("apiservice")
    .WaitFor(postgresdb)
    .WithReference(postgresdb)
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health");

await builder.Build().RunAsync();
