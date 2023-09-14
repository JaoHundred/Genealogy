using Avalonia.Controls;
using GeneA.ViewModels;

namespace GeneA.Views
{
    public partial class PeopleView : UserControl
    {

        public PeopleView()
        {
            InitializeComponent();
        }

        public PeopleView(PeopleViewModel peopleViewModel)
        {
            InitializeComponent();
            DataContext = peopleViewModel;
        }
    }
}
