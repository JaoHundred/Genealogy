using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelA.Core
{
    public class Nationality : IDbEntity
    {
        public Nationality() { }

        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Abbreviation { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }

        public override string ToString()
        {
            return $"{Name} {Abbreviation}";
        }
    }
}
