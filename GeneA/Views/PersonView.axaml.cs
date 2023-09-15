using Avalonia.Controls;
using GeneA.ViewModels;

namespace GeneA.Views;

public partial class PersonView : UserControl
{
    public PersonView()
    {
        InitializeComponent();
    }

    public PersonView(PersonViewModel personViewModel)
    {
        InitializeComponent();
        DataContext = personViewModel;
    }
}
