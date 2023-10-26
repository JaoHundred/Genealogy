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

    }
}
