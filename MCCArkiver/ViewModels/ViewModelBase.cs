using System.Collections.ObjectModel;
using MCCArkiver.Managers;
using MCCArkiver.Models;
using ReactiveUI;

namespace MCCArkiver.ViewModels;

public class ViewModelBase : ReactiveObject
{
    public GameCollection Games => GameManager.GameCollection;
    public LogCollection Log => Shisno.Log;
    public Progress Prog => Shisno.Prog;
    public string Version => Main.Version;
    public bool IsLinux => Main.IsLinux;
    public bool HasDesktop => Main.HasDesktop;
    public Conf Conf => Main.Conf;

    private int _winHeight = 700;
    public int WinHeight
    {
        get => _winHeight;
        set
        {
            this.RaiseAndSetIfChanged(ref _winHeight, value);
        }
    }

}