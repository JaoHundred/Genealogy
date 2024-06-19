using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Interfaces
{
    public interface IDbEntity
    {
        public Guid Id { get; set; }

        public DateTime CreatedDate { get; }
    }
}
