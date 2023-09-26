using Model.Interfaces;
using ModelA.Enums;
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
        {
            Spouse = new List<Person>();
            Offsprings = new List<Person>();
        }

        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public GenderEnum.Gender Gender { get; set; }
        public Nationality? Nacionality { get; set; }
        public Person? Father { get; set; }
        public Person? Mother { get; set; }
        public List<Person> Spouse { get; set; }
        public List<Person> Offsprings { get; set; }
        public DateTime? WeddingDate { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? DeathDate { get; set; }
        public DateTime? BaptismDate { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
