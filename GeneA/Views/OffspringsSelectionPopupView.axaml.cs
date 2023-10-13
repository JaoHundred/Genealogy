using Avalonia.Controls;
using Avalonia.Input;
using GeneA.Interfaces;
using GeneA.ViewModels;
using System;

namespace GeneA.Views
{
    public partial class OffspringsSelectionPopupView : UserControl, IDisposable, IPopupView
    {
        public OffspringsSelectionPopupView()
        {
            InitializeComponent();
            CancelButton.Tapped += CancelButton_Tapped;
            ConfirmButton.Tapped += ConfirmButton_Tapped;
        }

        private void ConfirmButton_Tapped(object? sender, TappedEventArgs e)
        {
            Dispose();
        }

        private void CancelButton_Tapped(object? sender, TappedEventArgs e)
        {
            Dispose();
        }

        public void Dispose()
        {
            IsVisible = false;

            CancelButton.Tapped -= CancelButton_Tapped;
            ConfirmButton.Tapped -= ConfirmButton_Tapped;
        }
    }
}
