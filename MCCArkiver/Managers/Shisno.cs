using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using DynamicData;
using MCCArkiver.Models;

namespace MCCArkiver.Managers;

public class Shisno
{
    public static LogCollection Log = new();
    public static Progress Prog = new();

    public static void SetProg(int i)
    {
        Prog.Max = i;
    }

    public static void AddProg(int i, string? info)
    {
        Prog.Val = i;
        Prog.Info = info;
        // Status(i.ToString());
    }

    public static void Error(string msg, Exception e)
    {
        Log.Logs.Add(new LogModel()
        {
            Title = $"{msg}: {e.Message}",
            Message = $"{e.StackTrace}",
            Level = 0,
            IsError = true
        });
        Log.Last = Log.Logs.LastOrDefault();
        Log.Status = $"{msg}: {e.Message}";
        Log.Errored = true;
    }

    public static void Alert(string title, string msg)
    {
        Log.Logs.Add(new LogModel()
        {
            Title = $"{title}",
            Message = $"{msg}",
            Level = 1,
            IsAlert = true
        });
        Log.Last = Log.Logs.LastOrDefault();
        Log.Status = $"ALERT! {title}";
        Log.Errored = true;
    }
    public static void Debug(string title, string msg)
    {
        Log.Logs.Add(new LogModel()
        {
            Title = $"{title}",
            Message = $"{msg}",
            Level = 4,
            IsDebug = true
        });
        Log.Last = Log.Logs.LastOrDefault();
        // Log.Status = $"ALERT! {title}";
        Log.Errored = true;
    }

    public static void Update(string title, string msg)
    {
        Log.Logs.Add(new LogModel()
        {
            Title = $"{title}",
            Message = $"{msg}",
            Level = 1,
            IsUpdate = true
        });
        Log.Last = Log.Logs.LastOrDefault();
        Log.Status = $"{title}";
        Log.Errored = true;
        Main.Notify(msg);
    }

    public static void Status(string msg)
    {
        Log.Status = msg;
    }

    public static void Note(string msg)
    {
        Log.Logs.Add(new LogModel()
        {
            Message = msg,
            Level = 3,
            IsNote = true
        });
        Log.Last = Log.Logs.LastOrDefault();
        Log.Status = msg;
    }
    public static void SoftAlert(string msg)
    {
        Log.Logs.Add(new LogModel()
        {
            Message = msg,
            Level = 2,
            IsSoftAlert = true
        });
        Log.Last = Log.Logs.LastOrDefault();
        Log.Status = msg;
    }
    public static void Good(string msg)
    {
        LogModel log;
        try
        {
            log = new LogModel()
            {
                Message = msg,
                Level = 2,
                IsGood = true
            };
            Log.Logs.Add(log);
            Log.Last = Log.Logs.LastOrDefault();
            Log.Status = msg;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{msg}");
            // throw;
        }
    }
}

public class Progress : INotifyPropertyChanged
{
    private int _max = 0;
    public int Max
    {
        get => _max;
        set
        {
            _max = value;
            OnPropertyChanged();
        }
    }

    private int _val = 0;
    public int Val
    {
        get => _val;
        set
        {
            _val = value;
            OnPropertyChanged();
        }
    }

    private string? _info;

    public string? Info
    {
        get => _info;
        set
        {
            _info = value;
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

public class LogCollection : INotifyPropertyChanged
{
    private string _status = "test";
    public string Status
    {
        get => _status;
        set
        {
            _status = value;
            OnPropertyChanged();
        }
    }

    private int _logLevel = 3;

    public int LogLevel
    {
        get => _logLevel;
        set
        {
            _logLevel = value;
            OnPropertyChanged();
        }
    }

    private bool _errored;

    public bool Errored
    {
        get => _errored;
        set
        {
            _errored = value;
            OnPropertyChanged();
        }
    }

    private LogModel? _last;

    public LogModel? Last
    {
        get => _last;
        set
        {
            _last = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<LogModel> _logs = new();
    public ObservableCollection<LogModel> Logs
    {
        get => _logs;
        set
        {
            _logs = value;
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