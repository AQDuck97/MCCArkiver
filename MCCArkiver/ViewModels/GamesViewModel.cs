using System;
using MCCArkiver.Managers;

namespace MCCArkiver.ViewModels;

public class GamesViewModel : ViewModelBase
{
    public int MaxHeight => (int)(WinHeight * 0.8);
    public string SomeString => "yo";

    public void Import(string file)
    {
        try
        {
            Shisno.Good("Sent film to game");
        }
        catch (Exception e)
        {
            Shisno.Error("Failed to send film", e);
        }
    }
}