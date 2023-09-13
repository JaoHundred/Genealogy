using Avalonia.Controls;
using GeneA.Interfaces;
using GeneA.ViewModels;

namespace GeneA.Views
{
    public partial class SettingsView : UserControl, IMenuView
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        public SettingsView(SettingsViewModel settingsViewModel)
        {
            InitializeComponent();
            DataContext = settingsViewModel;
        }
    }
}
