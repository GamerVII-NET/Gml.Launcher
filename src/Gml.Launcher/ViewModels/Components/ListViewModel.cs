﻿using System.Collections.ObjectModel;
using System.Linq;
using Gml.Launcher.ViewModels.Base;
using Gml.WebApi.Models.Dtos.Profiles;
using ReactiveUI;

namespace Gml.Launcher.ViewModels.Components;

public class ListViewModel : ViewModelBase
{

    public ObservableCollection<ReadProfileDto>? Profiles
    {
        get => _profiles;
        set
        {
            this.RaiseAndSetIfChanged(ref _profiles, value);
            this.RaisePropertyChanged(nameof(IsNotLoaded));
            this.RaisePropertyChanged(nameof(HasItems));
            this.RaisePropertyChanged(nameof(HasSelectedItem));
        }
    }

    public ReadProfileDto? SelectedProfile
    {
        get => _selectedProfile;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedProfile, value);
            this.RaisePropertyChanged(nameof(HasSelectedItem));
        }
    }

    public bool IsNotLoaded => _profiles == null;
    public bool HasItems => _profiles != null && _profiles.Any();
    public bool HasSelectedItem => _selectedProfile != null;


    private ObservableCollection<ReadProfileDto>? _profiles;
    private ReadProfileDto? _selectedProfile;

}
