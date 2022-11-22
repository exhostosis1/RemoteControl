using System;

namespace Shared;

public interface IConfigProvider
{
    public Uri ConfigUri { get; set; }
}