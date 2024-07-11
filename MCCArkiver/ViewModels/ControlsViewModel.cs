using System;
using System.IO;
using Avalonia;
using MCCArkiver.Managers;
using MCCArkiver.Models;
using ReactiveUI;

namespace MCCArkiver.ViewModels;

public class ControlsViewModel : ViewModelBase
{
    private bool _autoScan;

    public bool AutoScan
    {
        get => _autoScan;
        set => this.RaiseAndSetIfChanged(ref _autoScan, value);
    }

    public void ToggleScan()
    {
        if (AutoScan)
            ArchiveManager.TimerStart();
        else
            ArchiveManager.TimerStop();
    }
    
    public async void QuickScan()
    {
        try
        {
            await ArchiveManager.CheckSource();
        }
        catch (Exception e)
        {
            Shisno.Error("Failed quickscan", e);
        }
    }

    public void ArkScan()
    {
        try
        {
            ArchiveManager.GetGames();
        }
        catch (Exception e)
        {
            Shisno.Error("Failed to get games from arkive", e);
        }
    }

    public void SaveGames()
    {
        try
        {
            Main.SaveJson(GameManager.GameCollection, Main.Conf.ArchivePath);
        }
        catch (Exception e)
        {
            Shisno.Error("Failed to save games to json", e);
        }
    }

    public void LoadGames()
    {
        try
        {
            GameManager.ReadGames();
        }
        catch (Exception e)
        {
            Shisno.Error("Failed to read games", e);
        }
    }

    public void Legacy()
    {
        if(!string.IsNullOrEmpty(Conf.LegacyArk))
            ArchiveManager.LegacyProbe(Conf.LegacyArk);
    }

    public void Exit()
    {
        Environment.Exit(0);
    }
}