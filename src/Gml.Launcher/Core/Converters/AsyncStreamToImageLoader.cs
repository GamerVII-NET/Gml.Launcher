﻿using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media.Imaging;
using Gml.Launcher.Views.Components;

namespace Gml.Launcher.Core.Converters;

public class AsyncStreamToImageLoader
{
    public static readonly AttachedProperty<string> SourceProperty =
        AvaloniaProperty.RegisterAttached<AsyncStreamToImageLoader, BackgroundComponent, string>("Source");

    static AsyncStreamToImageLoader()
    {
        SourceProperty.Changed.AddClassHandler<BackgroundComponent>(OnSourceChanged);

        TempPath = Path.GetTempPath();
    }

    public static string TempPath { get; set; }

    private static async void OnSourceChanged(BackgroundComponent sender, AvaloniaPropertyChangedEventArgs args)
    {
        var url = args.GetNewValue<string>();

        if (string.IsNullOrEmpty(url))
        {
            sender.Source = null;
            return;
        }

        if (string.IsNullOrEmpty(url) || !ValidateUrl(url))
        {
            sender.Source = null;
            return;
        };

        var fileName = Path.Combine(TempPath, Path.GetFileName(url));

        if (!File.Exists(fileName))
        {
            using var client = new HttpClient();
            var response = await client.GetByteArrayAsync(url);
            using var stream = new MemoryStream(response);
            await ConvertStreamToFile(stream, fileName);
        }

        sender.Source = new Bitmap(File.OpenRead(fileName));
    }

    private static async Task ConvertStreamToFile(Stream input, string filePath)
    {
        var fileInfo = new FileInfo(filePath);

        if (!fileInfo.Directory!.Exists)
        {
            fileInfo.Directory.Create();
        }

        using (var fileStream = File.Create(filePath))
        {
            await input.CopyToAsync(fileStream);
        }
    }

    private static bool ValidateUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)
               && Path.GetFileName(url) is {} fileName && Guid.TryParse(fileName, out _);
    }

    public static void SetSource(BackgroundComponent obj, string value) => obj.SetValue(SourceProperty, value);
    public static string GetSource(BackgroundComponent obj) => obj.GetValue(SourceProperty);
}
