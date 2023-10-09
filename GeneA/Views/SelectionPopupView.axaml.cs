using Avalonia.Controls;
using Avalonia.Input;
using GeneA.Interfaces;
using System;

namespace GeneA.Views
{
    public partial class SelectionPopupView : UserControl, IDisposable, IPopup
    {
        public SelectionPopupView()
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
