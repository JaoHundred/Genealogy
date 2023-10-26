using CommunityToolkit.Mvvm.ComponentModel;
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

            //TODO: translate here the enum name
            return enumName ?? string.Empty;
        }
    }

    public enum FilterType
    {
        Birth,
        Death,
        BirthDeath,
        Baptism,
        Wedding,
        HasParents,
        HasChildren,
        HasSpouse,
        Gender,
        Nacionality
    }
}
