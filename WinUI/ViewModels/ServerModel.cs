﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using MainApp.Interfaces;
using MainApp.Servers;

namespace WinUI;

internal partial class ServerModel: ObservableObject
{
    public string Name { get; set; }
    public ServerType Type { get; set; }
    public Guid Id { get; set; }

    [ObservableProperty] private bool _status;

    public bool IsAutostart { get; set; }
    public Uri ListeningUri { get; set; }
    public Uri ApiUri { get; set; }
    public string ApiKey { get; set; }
    public IReadOnlyList<string> Usernames { get; set; }
}