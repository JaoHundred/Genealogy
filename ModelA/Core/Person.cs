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
            Spouses = new List<Person>();
            Offsprings = new List<Person>();
        }

        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public GenderEnum.Gender Gender { get; set; }
        public Nationality? Nacionality { get; set; }
        public Person? Father { get; set; }
        public Person? Mother { get; set; }
        public List<Person> Spouses { get; set; }
        public List<Person> Offsprings { get; set; }
        public DateTime? WeddingDate { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? DeathDate { get; set; }
        public DateTime? BaptismDate { get; set; }
        public string Description { get; set; } = string.Empty;


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Name);
            sb.Append(" (");

            if (BirthDate == null)
                sb.Append("?/?/?");
            else
                sb.Append(BirthDate.Value.ToString("d"));

            sb.Append(" - ");

            if (DeathDate == null)
                sb.Append("?/?/?");
            else
                sb.Append(DeathDate.Value.ToString("d"));

            sb.Append(' ');
            sb.Append(char.ConvertFromUtf32(0x271D));//cross sign ✝
            sb.Append(')');

            return sb.ToString();
        }
    }
}
