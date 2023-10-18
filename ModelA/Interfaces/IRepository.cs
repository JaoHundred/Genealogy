using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Interfaces
{
    public interface IRepository<T> where T : IDbEntity
    {
        IEnumerable<T> Take(int amount, bool takeLast = true);

        IEnumerable<T> FindAll();
        T FindById(long id);
        T Upsert(T entity);
        void Delete(T entity);
        void Update(T entity);
    }
}
