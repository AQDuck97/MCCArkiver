using Avalonia.Controls;
using MCCArkiver.Managers;

namespace MCCArkiver.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Main.Win = this;
    }
}