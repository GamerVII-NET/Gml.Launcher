﻿using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Gml.Launcher.Views.Components;

public class GmlButton : TemplatedControl
{

    public static readonly StyledProperty<string> IconPathProperty = AvaloniaProperty.Register<GmlButton, string>(
        nameof(IconPath), "/Assets/Images/profile.svg");

    public static readonly StyledProperty<double> IconSizeProperty = AvaloniaProperty.Register<GmlButton, double>(
        nameof(IconSize), 16);

    public static readonly StyledProperty<ICommand> CommandProperty = AvaloniaProperty.Register<GmlButton, ICommand>(
        "Command");

    public static readonly StyledProperty<string?> TextProperty = AvaloniaProperty.Register<GmlButton, string?>(
        nameof(Text), "DefaultButton style");

    public static readonly StyledProperty<int> SpacingProperty = AvaloniaProperty.Register<GmlButton, int>(
        nameof(Spacing), 10);

    public int Spacing
    {
        get => GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public ICommand Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public double IconSize
    {
        get => GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    public string IconPath
    {
        get => GetValue(IconPathProperty);
        set => SetValue(IconPathProperty, value);
    }

}

