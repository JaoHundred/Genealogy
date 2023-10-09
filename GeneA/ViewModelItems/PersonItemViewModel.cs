using CommunityToolkit.Mvvm.ComponentModel;
using Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneA.ViewModelItems
{
    [ObservableObject]
    public partial class PersonItemViewModel : Person
    {
        [ObservableProperty]
        private bool _isSelected;
    }
}
