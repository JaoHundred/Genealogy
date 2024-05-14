using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform;
using System;
using System.Linq;

namespace GeneA.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        ScreenResolution();
        InitializeComponent();
    }

    private void ScreenResolution()
    {
        var primaryScreen = Screens.All.FirstOrDefault();

        if (primaryScreen != null)
        {
            double height = primaryScreen.Bounds.Height;
            double width = primaryScreen.Bounds.Width;

            Height = height / 3;
            Width = width / 3;
        }
    }
}
