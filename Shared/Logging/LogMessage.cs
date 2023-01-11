using System;
using Shared.Enums;

namespace Shared.Logging;

public record LogMessage(Type CallerType, LoggingLevel Level, DateTime Date, string Message);