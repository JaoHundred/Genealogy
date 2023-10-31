using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;
using GeneA.Interfaces;
using GeneA.ViewModelItems;
using GeneA.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace GeneA.Views
{
    public partial class PersonListingView : UserControl, IMenuView
    {

        public PersonListingView()
        {
            InitializeComponent();
        }

        //bellow events make splitview dont close when its not supposed to in controls which have
        //some sort of dialog part(Combobox and CalendarDatePicker in this case)

        private void ComboBox_DropDownClosed(object? sender, System.EventArgs e)
        {
            _lostFocusByControlClosed = true;
        }

        private void SplitView_PaneClosed(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_lostFocusByControlClosed)
            {
                splitPane.IsPaneOpen = true;
                _lostFocusByControlClosed = false;
            }
        }

        private bool _lostFocusByControlClosed;
        private void CalendarDatePicker_CalendarClosed(object? sender, System.EventArgs e)
        {
            _lostFocusByControlClosed = true;
        }

    }
}
