using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Rydet.IOBased.Tests;


[SuppressMessage("Design", "CA1034:Nested types should not be visible")]
public sealed class XunitLogProvider : ILoggerProvider
{
    
    private readonly ITestOutputHelper _helper;
    private readonly Func<SimpleLogEntry, string> _message;

    public XunitLogProvider(ITestOutputHelper helper, Func<SimpleLogEntry, string> message)
    {
        _helper = helper;
        _message = message;
    }

    public void Dispose() { }

    public ILogger CreateLogger(string categoryName) => new XunitLogger(_helper, categoryName, _message);

    public class SimpleLogEntry(EventId eventId, string logLevel, string categoryName, string formattedString)
    {
        public EventId EventId { get; } = eventId;
        public string LogLevel { get; } = logLevel;
        public string CategoryName { get; } = categoryName;
        public string FormattedString { get; } = formattedString;
    }


    private sealed class XunitLogger : ILogger
    {
        private readonly ITestOutputHelper _outputHelper;
        private readonly string _categoryName;
        private readonly Func<SimpleLogEntry, string> _message;

        public XunitLogger(ITestOutputHelper outputHelper, string categoryName, Func<SimpleLogEntry, string> message)
        {
            _outputHelper = outputHelper;
            _message = message;
            _categoryName = categoryName;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null!;

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            ArgumentNullException.ThrowIfNull(formatter);
            var ll = LogLevel_(logLevel);
            var simpleLogEntry = new SimpleLogEntry(eventId, ll, formatter(state, exception), _categoryName);
            _outputHelper.WriteLine(_message(simpleLogEntry));

        }

        private static string LogLevel_(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Trace => "trce",
                LogLevel.Debug => "dbug",
                LogLevel.Information => "info",
                LogLevel.Warning => "warn",
                LogLevel.Error => "fail",
                LogLevel.Critical => "crit",
                _ => throw new ArgumentOutOfRangeException(nameof(logLevel))
            };
        }
    }
}
