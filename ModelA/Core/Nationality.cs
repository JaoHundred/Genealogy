using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Core
{
    public class Nationality : IDbEntity
    {
        public Nationality() { }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
    }
}
