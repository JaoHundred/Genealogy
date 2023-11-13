using CommunityToolkit.Mvvm.ComponentModel;
using GeneA._Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GeneA._Helper.AppStateEnums;

namespace GeneA.ViewModelItems
{
    public partial class AppThemeItemViewModel : ObservableObject
    {
        public AppTheme AppTheme { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
