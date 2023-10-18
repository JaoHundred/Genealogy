using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Database
{
    public class Repository<T> : IRepository<T> where T : IDbEntity
    {

        public Repository(LiteDBConfiguration configuration)
        {
            _configuration = configuration;
        }

        private readonly LiteDBConfiguration _configuration;

        public IEnumerable<T> FindAll()
        {
            return _configuration.LiteDB!.GetCollection<T>().FindAll();
        }

        public T FindById(long id)
        {
            return _configuration.LiteDB!.GetCollection<T>().FindById(id);
        }

        public T Upsert(T entity)
        {
            _configuration.LiteDB!.GetCollection<T>().Upsert(entity);
            return FindById(entity.Id);
        }

        public void Delete(T entity)
        {
            _configuration.LiteDB!.GetCollection<T>().Delete(entity.Id);
        }

        public void Update(T entity)
        {
            _configuration.LiteDB!.GetCollection<T>().Update(entity.Id, entity);
        }

        public IEnumerable<T> Take(int amount, bool takeLast = true)
        {
            var collection = _configuration.LiteDB!.GetCollection<T>().FindAll();

            if (takeLast)
                return collection.TakeLast(amount).Reverse();

            return collection.Take(amount);
        }

      
    }
}
