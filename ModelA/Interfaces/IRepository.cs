using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Interfaces
{
    public interface IRepository<T> where T : IDbEntity
    {
        T FindById(long id);
        void Upsert(T entity);
        void Delete(T entity);
        void Update(T entity);
    }
}
