using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace MCCArkiver.Models;

public class Conf : INotifyPropertyChanged
{
    private string _gamerTag = "";
    public string GamerTag
    {
        get => _gamerTag;
        set
        {
            _gamerTag = value;
            OnPropertyChanged();
        }
    }

    private string? _prefix;
    public string? Prefix
    {
        get => _prefix;
        set
        {
            _prefix = value;
            OnPropertyChanged();
        }
    }

    private string _arkive = "";
    public string ArchivePath
    {
        get => _arkive;
        set
        {
            _arkive = value;
            OnPropertyChanged();
        }
    }

    private string _legacyArk = "";

    public string LegacyArk
    {
        get => _legacyArk;
        set
        {
            _legacyArk = value;
            OnPropertyChanged();
        }
    }

    private string _updateUrl = "aqduck97/mccarkiver";
    public string UpdateUrl
    {
        get => _updateUrl;
        set
        {
            _updateUrl = value;
            OnPropertyChanged();
        }
    }

    private bool _autoUpdate;
    public bool AutoUpdate
    {
        get => _autoUpdate;
        set
        {
            _autoUpdate = value;
            OnPropertyChanged();
        }
    }

    private bool _isInstalled;
    [JsonIgnore]
    public bool IsInstalled
    {
        get => _isInstalled;
        set
        {
            _isInstalled = value;
            OnPropertyChanged();
        }
    }

    private bool _timer;
    [JsonIgnore]
    public bool Timer
    {
        get => _timer;
        set
        {
            _timer = value;
            OnPropertyChanged();
        }
    }
    
    
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}