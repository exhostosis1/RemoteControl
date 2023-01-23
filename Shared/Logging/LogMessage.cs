using Shared.Enums;
using System;

namespace Shared.Logging;

public record LogMessage(Type CallerType, LoggingLevel Level, DateTime Date, string Message);