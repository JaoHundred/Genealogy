using CommunityToolkit.Mvvm.ComponentModel;
using GeneA.Interfaces;
using ModelA.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneA.ViewModelItems
{
    [ObservableObject]
    public partial class NationalityItemViewModel : Nationality, ISelectable
    {
        [ObservableProperty]
        private bool? _isSelected;
    }
}
