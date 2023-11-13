using Avalonia.Automation.Provider;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using GeneA.ViewModels;

namespace GeneA.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (TopLevel.GetTopLevel(this) is { } topLevel)
        {
            topLevel.BackRequested += TopLevel_BackRequested; 
        }
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        if (TopLevel.GetTopLevel(this) is { } topLevel)
        {
            topLevel.BackRequested -= TopLevel_BackRequested; 
        }
        base.OnUnloaded(e);
    }

    private void TopLevel_BackRequested(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel mainViewModel && mainViewModel.GoBackCommand.CanExecute(null))
        {
            mainViewModel.GoBackCommand.Execute(null);
            e.Handled = true;
        }
    }
}
