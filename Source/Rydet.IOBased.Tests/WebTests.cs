using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Rydet.IOBased.Tests;

public sealed class WebTests(ITestOutputHelper helper)
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

    [Fact]
    public async Task GetWebResourceRootReturnsOkStatusCodeAsync()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource(DefaultTimeout);
        var cancellationToken = cancellationTokenSource.Token;

        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Rydet_AppHost>(cancellationToken);
        appHost.Services.AddLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Debug);
            // Override the logging filters from the app's configuration
            logging.AddFilter(appHost.Environment.ApplicationName, LogLevel.Debug);
            logging.AddFilter("Aspire.", LogLevel.Debug);
            // To output logs to the xUnit.net ITestOutputHelper, consider adding a package from https://www.nuget.org/packages?q=xunit+logging
            logging.AddProvider(new XunitLogProvider(helper, l => $"""
                                                                     {l.LogLevel}: {l.CategoryName}[{l.EventId}]
                                                                           {l.FormattedString}
                                                                     """));
        });

        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        await using var app = await appHost.BuildAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
        await app.StartAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);

        // Act
        using var httpClient = app.CreateHttpClient("apiservice");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("apiservice", cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
        using var response = await httpClient.GetAsync(new Uri("/", UriKind.Relative), cancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
