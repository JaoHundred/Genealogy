using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GeneA._Helper
{
    public static class RegexHelper
    {
        public static Match CanAddNationality(string searchText)
        {
            return Regex.Match(searchText, @"^([a-zA-Z]+(?: [a-zA-Z]+)*) ([A-Z]+)$"); //"{anything}{space}{anything in uppercase}"
        }
    }
}
