using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using MCCArkiver.Managers;

namespace MCCArkiver.Models;

public class Match : INotifyPropertyChanged
{
    private string _gameSource;
    public string Game { get; set; }
    public string Title { get; set; }
    public string Date { get; set; }
    public string Duration { get; set; }
    public string FilePath { get; set; }

    private bool _InGame;

    [JsonIgnore]
    public bool InGame
    {
        get => _InGame;
        set
        {
            _InGame = value;
            OnPropertyChanged();
        }
    }

    public void GameCheck()
    {
        _gameSource = $"{Main.UserContent}/UserContent/{GamePath()}/Movie/{Path.GetFileName(FilePath)}";
        InGame = File.Exists(_gameSource);
    }

    public void Import()
    {
        try
        {
            File.Copy(FilePath, _gameSource);
            Shisno.Good($"Copied to game");
            InGame = true;
        }
        catch (Exception e)
        {
            Shisno.Error("Failed to copy file", e);
        }
    }

    public void Remove()
    {
        File.Delete(_gameSource);
        Shisno.Good($"Removed film from game");
        InGame = false;
    }

    private string GamePath()
    {
        switch (Game.ToLower())
        {
            case "halo3":
                return "Halo3";
            case "halo2c":
                return "Halo2";
            case "haloreach":
                return "HaloReach";
            case "halo2a":
                return "Halo2A";
            case "halo4":
                return "Halo4";
            case "halo3odst":
                return "Halo3ODST";
        }

        return Game;
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