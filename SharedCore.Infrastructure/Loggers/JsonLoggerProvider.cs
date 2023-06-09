﻿using System.Collections.Concurrent;

using Microsoft.Extensions.Logging;

namespace SharedCore.Infrastructure.Loggers
{
    public sealed class JsonLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, JsonLogger> _loggers = new(StringComparer.OrdinalIgnoreCase);

        public ILogger CreateLogger(string categoryName) =>
            _loggers.GetOrAdd(categoryName, name => new JsonLogger(name));

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}