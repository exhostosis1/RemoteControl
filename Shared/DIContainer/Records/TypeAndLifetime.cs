using System;
using Shared.Enums;

namespace Shared.DIContainer.Records;

public record TypeAndLifetime(Type Type, Delegate? Constructor, Lifetime Lifetime);