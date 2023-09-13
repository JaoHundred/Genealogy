using Avalonia.Controls;
using GeneA.Interfaces;
using GeneA.ViewModels;

namespace GeneA.Views
{
    public partial class HomeView : UserControl, IMenuView
    {
        public HomeView()
        {
            InitializeComponent();
        }

        public HomeView(HomeViewModel homeViewModel)
        {
            InitializeComponent();
            DataContext = homeViewModel;
        }
    }
}
