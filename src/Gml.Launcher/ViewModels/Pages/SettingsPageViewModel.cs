﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Gml.Launcher.Core.Services;
using Gml.Launcher.Models;
using Gml.Launcher.ViewModels.Base;
using Gml.Web.Api.Dto.Profile;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Gml.Launcher.ViewModels.Pages;

public class SettingsPageViewModel : PageViewModelBase
{
    private readonly IStorageService _storageService;

    [Reactive]
    public bool DynamicRamValue { get; set; }

    [Reactive]
    public bool FullScreen { get; set; }

    [Reactive]
    public ulong MaxRamValue { get; set; }

    [Reactive]
    public Language? SelectedLanguage { get; set; }


    public double RamValue
    {
        get => _ramValue;
        set
        {
            if (!(Math.Abs(value - _ramValue) > 0.0)) return;

            _ramValue = RoundToNearest(value, 8);
            this.RaisePropertyChanged();
        }
    }

    public string WindowWidth
    {
        get => _windowWidth.ToString();
        set => this.RaiseAndSetIfChanged(ref _windowWidth, int.Parse(string.Concat(value.Where(char.IsDigit))));
    }

    public string WindowHeight
    {
        get => _windowHeight.ToString();
        set => this.RaiseAndSetIfChanged(ref _windowHeight, int.Parse(string.Concat(value.Where(char.IsDigit))));
    }

    public ObservableCollection<Language> AvailableLanguages { get; }

    private int _windowWidth;
    private int _windowHeight;
    private double _ramValue;

    public SettingsPageViewModel(
        IScreen screen,
        ILocalizationService? localizationService,
        IStorageService storageService,
        ISystemService systemService,
        ProfileReadDto selectedProfile) : base(screen, localizationService)
    {
        _storageService = storageService;

        this.WhenAnyValue(
                x => x.RamValue,
                x => x.WindowWidth,
                x => x.WindowHeight,
                x => x.FullScreen,
                x => x.DynamicRamValue,
                x => x.SelectedLanguage
            )
            .Throttle(TimeSpan.FromMilliseconds(400))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(SaveSettings);

        this.WhenAnyValue(x => x.SelectedLanguage)
            .Skip(2)
            .Subscribe(language =>
            {
                if (language == null) return;

                Assets.Resources.Resources.Culture = language.Culture;

                GoBackCommand.Execute(null);
            });


        AvailableLanguages = new ObservableCollection<Language>(systemService.GetAvailableLanguages());

        MaxRamValue = systemService.GetMaxRam();

        RxApp.MainThreadScheduler.Schedule(LoadSettings);
    }

    private double RoundToNearest(double value, double step)
    {
        var offset = value % step;
        return offset >= step / 2.0
            ? value + (step - offset)
            : value - offset;
    }

    private async void LoadSettings()
    {
        var data = await _storageService.GetAsync<SettingsInfo>(StorageConstants.Settings);

        if (data == null) return;

        if (!string.IsNullOrEmpty(data.LanguageCode))
        {
            SelectedLanguage = AvailableLanguages.FirstOrDefault(c => c.Culture.Name == data.LanguageCode);
        }
        else
        {
            SelectedLanguage = AvailableLanguages.FirstOrDefault();
        }

        WindowWidth = data.GameWidth == 0 ? "900" : data.GameWidth.ToString();
        WindowHeight = data.GameHeight == 0 ? "600" : data.GameHeight.ToString();
        RamValue = data.RamValue;
        DynamicRamValue = data.IsDynamicRam;
        FullScreen = data.FullScreen;
    }

    private void SaveSettings(
        (double ramValue, string width, string height, bool isFullScreen, bool isDynamicRam, Language? selectedLanguage) update)
    {
        _storageService.SetAsync(
            StorageConstants.Settings,
            new SettingsInfo(
                int.Parse(update.width),
                int.Parse(update.height),
                update.isFullScreen,
                update.isDynamicRam,
                update.ramValue, update.selectedLanguage?.Culture.Name));
    }
}
