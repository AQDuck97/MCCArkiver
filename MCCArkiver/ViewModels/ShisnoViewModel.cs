using System;
using System.Collections.ObjectModel;
using System.Linq;
using MCCArkiver.Managers;
using MCCArkiver.Models;
using ReactiveUI;

namespace MCCArkiver.ViewModels;

public class ShisnoViewModel : ViewModelBase
{
    public int MaxHeight => (int)(WinHeight * 0.8);
    
    private bool _autoScroll = true;
    public bool AutoScroll
    {
        get => _autoScroll;
        set => this.RaiseAndSetIfChanged(ref _autoScroll, value);
    }

    public void ClearLogs()
    {
        Log.Logs.Clear();
    }

    public void DismissStatus()
    {
        Log.Status = "";
    }

    public void Delete(long unix)
    {
        try
        {
            LogModel log = Log.Logs.FirstOrDefault(l => l.Timestamp == unix);
            Log.Logs.Remove(log);
        }
        catch (Exception e)
        {
            Shisno.Error("Failed to remove log", e);
        }
    }
}