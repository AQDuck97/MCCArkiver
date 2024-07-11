using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MCCArkiver.Models;

public class Game
{
    public string Name { get; set; }
    public ObservableCollection<Match>? Matches { get; set; } = new();
}

public class GameCollection : INotifyPropertyChanged
{
    private ObservableCollection<Game> _games = new();
    public ObservableCollection<Game> Games
    {
        get => _games;
        set
        {
            _games = value;
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