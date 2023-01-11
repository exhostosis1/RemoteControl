﻿using Autostart;
using ConfigProviders;
using ControlProviders;
using Logging;
using Shared;
using Shared.Config;
using Shared.ControlProviders;
using Shared.Logging;
using Shared.Logging.Interfaces;
using Shared.UI;
using WinFormsUI;

namespace WindowsEntryPoint;

public class RemoteControlContainer : IPlatformDependantContainer
{
    public IConfigProvider ConfigProvider { get; }
    public IAutostartService AutostartService { get; }
    public IUserInterface UserInterface { get; }
    public IAudioControlProvider AudioProvider { get; }
    public IDisplayControlProvider DisplayProvider { get; }
    public IKeyboardControlProvider KeyboardProvider { get; }
    public IMouseControlProvider MouseProvider { get; }
    public ControlFacade ControlProviders { get; }
    public ILogger Logger { get; }
    public ILogger NewLogger()
    {
#if DEBUG
        return new TraceLogger();
#else
        return new FileLogger(type, "error.log");
#endif
    }

    public IConfigProvider NewConfigProvider(ILogger logger) =>
        new LocalFileConfigProvider(new LogWrapper<LocalFileConfigProvider>(logger));

    public IAutostartService NewAutostartService(ILogger logger) =>
        new WinRegistryAutostartService(new LogWrapper<WinRegistryAutostartService>(logger));

    public IUserInterface NewUserInterface() => new MainForm();

    public IKeyboardControlProvider NewKeyboardProvider(ILogger logger) =>
        new User32Provider(new LogWrapper<User32Provider>(Logger));

    public IMouseControlProvider NewMouseProvider(ILogger logger) =>
        new User32Provider(new LogWrapper<User32Provider>(logger));

    public IDisplayControlProvider NewDisplayProvider(ILogger logger) =>
        new User32Provider(new LogWrapper<User32Provider>(logger));

    public IAudioControlProvider NewAudioProvider(ILogger logger) =>
        new NAudioProvider(new LogWrapper<NAudioProvider>(logger));

    public RemoteControlContainer()
    {
        Logger = NewLogger();
        ConfigProvider = NewConfigProvider(Logger);
        AutostartService = NewAutostartService(Logger);
        AudioProvider = NewAudioProvider(Logger);
        KeyboardProvider = NewKeyboardProvider(Logger);
        MouseProvider = NewMouseProvider(Logger);
        DisplayProvider = NewDisplayProvider(Logger);
        UserInterface = NewUserInterface();

        ControlProviders = new ControlFacade(AudioProvider, KeyboardProvider, MouseProvider, DisplayProvider);
    }
}