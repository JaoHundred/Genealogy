using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Core
{
    public class Person : IDbEntity
    {
        public Person()
        { }

        public long Id { get; set; }
        public string Name { get; set; }
        public Nacionality Nacionality { get; set; }
        public Person Father { get; set; }
        public Person Mother { get; set; }
        public Person Spouse { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime DeathDate { get; set; }
        public string Description { get; set; }
    }
}
