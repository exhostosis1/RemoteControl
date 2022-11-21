﻿namespace Shared;

public interface IContainer
{
    public IAutostartService AutostartService { get; }
    public IConfigProvider ConfigProvider { get; }
    public IServer Server { get; }
    public IUserInterface UserInterface { get; }
}