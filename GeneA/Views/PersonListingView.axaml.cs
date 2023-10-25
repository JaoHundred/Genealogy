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

        private void FilterListing_SelectionChanged(object? sender, TappedEventArgs e)
        {
            PersonListingView_Tapped(sender, e);
        }

        List<Border> borderList = new List<Border>();
        private void PersonListingView_Tapped(object? sender, Avalonia.Input.TappedEventArgs e)
        {
            if (sender is Border border)
            {
                if (borderList.Contains(border))
                {
                    border.Background = SolidColorBrush.Parse("Transparent");
                    borderList.Remove(border);
                }
                else
                {
                    border.Background = SolidColorBrush.Parse("Red");
                    borderList.Add(border);

                }
            }
        }
    }
}
