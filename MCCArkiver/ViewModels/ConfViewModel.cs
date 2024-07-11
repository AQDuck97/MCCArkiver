using System;
using System.IO;
using MCCArkiver.Managers;
using MCCArkiver.Models;
using ReactiveUI;

namespace MCCArkiver.ViewModels;

public class ConfViewModel : ViewModelBase
{
    private Conf? _conf;

    public Conf? TempConf
    {
        get => _conf;
        set => this.RaiseAndSetIfChanged(ref _conf, value);
    }

    public ConfViewModel()
    {
        TempConf = Main.Conf;
    }

    public void SaveConf()
    {
        try
        {
            Main.SaveConf(_conf);
        }
        catch (Exception e)
        {
            Shisno.Error("Failed to save conf", e);
        }
    }

    public void Reset()
    {
        try
        {
            TempConf = Main.ReadConf();
        }
        catch (Exception e)
        {
            Shisno.Error("Failed to read conf", e);
        }
    }

    public async void Browse(string dir)
    {
        try
        {
            switch (dir)
            {
                case "archive":
                    TempConf.ArchivePath = await Main.DirPicker(TempConf.ArchivePath);
                    break;
                case "prefix":
                    TempConf.Prefix = await Main.DirPicker(MediaCheck());
                    break;
                case "legacy":
                    TempConf.LegacyArk = await Main.DirPicker(TempConf.LegacyArk);
                    break;
            }
        }
        catch (Exception e)
        {
            Shisno.Error("DirPicker failed", e);
        }
    }

    public void CreateDesktop()
    {
        if (Main.HasDesktop)
        {
            File.Delete(Main.DesktopFile);
            Shisno.Good("Removed desktop file");
        }
        else
        {
            Main.CreateDesktop();
            Shisno.Good("Added desktop file");
        }

        Main.Conf.IsInstalled = !Main.Conf.IsInstalled;
    }

    private string MediaCheck()
    {
        if(Environment.UserName == "deck")
            return "/run/media/mmcblk0p1/steamapps/compatdata";
        
        string media = "/media";
        if (Directory.Exists(media))
            return media;
        
        return "/mnt";
    }
}