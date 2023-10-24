using GeneA.ViewModelItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneA._Helper
{
    public static class FilterHelper
    {
        public static List<FilterItemViewModel> FillFilters()
        {
            var filterEnums = Enum.GetValues<FilterType>();
            var filters = new List<FilterItemViewModel>();

            filters.AddRange(filterEnums.Select(p => new FilterItemViewModel { FilterType = p }));

            return filters;
        }
    }
}
