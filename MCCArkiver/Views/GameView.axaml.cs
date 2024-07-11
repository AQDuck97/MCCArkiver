using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MCCArkiver.ViewModels;

namespace MCCArkiver.Views;

public partial class GameView : UserControl
{
    public GameView()
    {
        InitializeComponent();
        DataContext = new GamesViewModel();
    }
}