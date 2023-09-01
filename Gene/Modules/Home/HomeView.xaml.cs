using Gene.Localization;
using Gene.Modules.Home;
using System.Linq;

namespace Gene
{
    public partial class HomeView : ContentPage
    {
        public HomeView(HomeViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private void ContentPage_Loaded(object sender, EventArgs e)
        {
            //collectionview emptylist workaround
            int? count = colView.ItemsSource?.Cast<object>().Count();

            if (count is null || count == 0)
                workaroundEmptyView.IsVisible = true;
        }
    }
}