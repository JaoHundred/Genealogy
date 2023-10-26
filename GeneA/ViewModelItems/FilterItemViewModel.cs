using CommunityToolkit.Mvvm.ComponentModel;
using GeneA._Helper;
using GeneA.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneA.ViewModelItems
{
    public partial class FilterItemViewModel : ObservableObject, ISelectable
    {
        public FilterType FilterType { get; set; }

        [ObservableProperty]
        private bool _isSelected;

        public override string ToString()
        {
            string? enumName = Enum.GetName(typeof(FilterType), FilterType);
            string name = enumName ?? string.Empty;
            
            return DynamicTranslate.Translate(name);
        }
    }

    public enum FilterType
    {
        BirthDate,
        DeathDate,
        Baptism,
        Wedding,
        HasParents,
        HasChildren,
        HasSpouse,
        Gender,
        Nationality
    }
}
