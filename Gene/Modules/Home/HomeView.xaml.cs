using Gene.Modules.Home;

namespace Gene
{
    public partial class HomeView : ContentPage
    {
        public HomeView(HomeViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}