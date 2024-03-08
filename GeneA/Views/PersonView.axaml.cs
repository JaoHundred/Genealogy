using Avalonia.Controls;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.DependencyInjection;
using GeneA.ViewModels;
using ModelA.Core;
using System;

namespace GeneA.Views;

public partial class PersonView : UserControl
{
    public PersonView()
    {
        InitializeComponent();
    }

    private void ButtonSpinner_Spin(object? sender, SpinEventArgs e)
    {
        ((PersonViewModel)DataContext!).SpinCommand.Execute(e.Direction.ToString());
    }

    private void Dgrid_CellPointerPressed(object? sender, DataGridCellPointerPressedEventArgs e)
    {
        if (e.Cell.DataContext is DocumentFile doc && e.Column.Header != null)
        {
            ((PersonViewModel)DataContext!).OpenFileCommand.Execute(doc);
        }
    }
}
