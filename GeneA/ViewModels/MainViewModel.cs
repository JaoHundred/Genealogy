using CommunityToolkit.Mvvm.ComponentModel;
using Model.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeneA.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        RecentlyAdded = new List<Person>();
    }

    [ObservableProperty]
    private List<Person> _recentlyAdded;

    public async Task AddNewPerson()
    {
        //TODO:navigate to AddPersonViewModel
        await Task.Delay(1000);
    }
}
