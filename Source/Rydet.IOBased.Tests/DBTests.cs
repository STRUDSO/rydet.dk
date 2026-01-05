using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Npgsql;
using Testcontainers.PostgreSql;
using Testcontainers.Xunit;
using Xunit.Abstractions;

namespace Rydet.IOBased.Tests;

[CollectionDefinition("Database collection")]
[SuppressMessage("Design", "MA0048:File name must match type name")]
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix")]
public class DatabaseFixtureCollection : ICollectionFixture<DatabaseFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}

[SuppressMessage("Design", "MA0048:File name must match type name")]
public sealed class DatabaseFixture(IMessageSink messageSink)
    : DbContainerFixture<PostgreSqlBuilder, PostgreSqlContainer>(messageSink)
{
    public override DbProviderFactory DbProviderFactory => NpgsqlFactory.Instance;

    protected override PostgreSqlBuilder Configure() => base.Configure().WithReuse(true);
}

[Collection("Database collection")]
[SuppressMessage("Design", "MA0048:File name must match type name")]
public class DBTests1
{
    private readonly DatabaseFixture _fixture;

    public DBTests1(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task METHOD()
    {
        var openConnectionAsync = await _fixture.OpenConnectionAsync();
        var dbCommand = openConnectionAsync.CreateCommand();
        dbCommand.CommandText = "SELECT 1";
        var executeScalar = await dbCommand.ExecuteScalarAsync();
        
        Assert.Equal(1, executeScalar);
    }

}[Collection("Database collection")]
[SuppressMessage("Design", "MA0048:File name must match type name")]
public class DBTests2
{
    private readonly DatabaseFixture _fixture;

    public DBTests2(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task METHOD()
    {
        var openConnectionAsync = await _fixture.OpenConnectionAsync();
        var dbCommand = openConnectionAsync.CreateCommand();
        dbCommand.CommandText = "SELECT 1";
        var executeScalar = await dbCommand.ExecuteScalarAsync();
        
        Assert.Equal(1, executeScalar);
    }
    
}
