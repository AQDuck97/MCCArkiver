using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MCCArkiver.Managers;
using MCCArkiver.Updater;
using ReactiveUI;

namespace MCCArkiver.ViewModels;

public class UpdateViewModel : ViewModelBase
{
    public string BtnTxt
    {
        get => _btnTxt;
        set => this.RaiseAndSetIfChanged(ref _btnTxt, value);
    }

    private string _btnTxt = "Check updates";

    public string Result
    {
        get => _result;
        set => this.RaiseAndSetIfChanged(ref _result, value);
    }

    private string _result = "";

    public List<ReleaseModel> Releases
    {
        get => _releases;
        set => this.RaiseAndSetIfChanged(ref _releases, value);
    }

    private List<ReleaseModel> _releases;

    private ReleaseManager _manager;

    public UpdateViewModel()
    {
        _manager = new();

        if (Conf.AutoUpdate)
            Check();
    }

    public async Task Check()
    {
        try
        {
            Result = "Checking...";
            Result = await Task.Run(() => _manager.CheckUpdates(Conf.UpdateUrl, Main.Version));
            Releases = _manager.Releases;
            
            if (_manager.NewVersion(Main.Version))
                Shisno.Update($"{Releases[0].Tag} available!", $"{Releases[0].Description}");
            else
                Shisno.Good($"Latest version ({Main.Version})");
        }
        catch (Exception e)
        {
            Shisno.Error("Failed update check", e);
        }
    }
}