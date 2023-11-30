using Avalonia.Controls;
using Avalonia.Interactivity;
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

    private void Dgrid_CellPointerPressed(object? sender, DataGridCellPointerPressedEventArgs e)
    {
        if(e.Cell.DataContext is DocumentFile doc && e.Column.Header != null)
        {
            ((PersonViewModel)DataContext!).OpenFileCommand.Execute(doc);
        }
    }
}
