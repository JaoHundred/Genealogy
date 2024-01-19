using Avalonia.Controls;
using Avalonia.Platform;
using Microsoft.CodeAnalysis;
using System.Runtime.InteropServices;

namespace GeneA.Views
{
    public partial class FamilyTreeView : UserControl
    {
        public FamilyTreeView()
        {
            InitializeComponent();

            
        }

        public FamilyTreeView(MainView mainView)
        {
            InitializeComponent();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                zoomBorder.PanButton = Avalonia.Controls.PanAndZoom.ButtonName.Left;
            }

            graphPanel.Initialized += (sender, args) =>
            {
                mainView._graphPanelInstance = graphPanel;
            };
        }
    }
}
