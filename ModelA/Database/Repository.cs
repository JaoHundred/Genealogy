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

        protected readonly LiteDBConfiguration _configuration;

        public virtual IEnumerable<T> FindAll()
        {
            return _configuration.LiteDB!.GetCollection<T>().FindAll();
        }

        public virtual T FindById(long id)
        {
            return _configuration.LiteDB!.GetCollection<T>().FindById(id);
        }

        public virtual T Upsert(T entity)
        {
            _configuration.LiteDB!.GetCollection<T>().Upsert(entity);
            return FindById(entity.Id);
        }

        public virtual void Delete(T entity)
        {
            _configuration.LiteDB!.GetCollection<T>().Delete(entity.Id);
        }

        public virtual async Task DeleteBatchAsync(IEnumerable<T> entities)
        {
            await Task.Run(() =>
            {
                foreach (T entity in entities)
                    Delete(entity);
            });
        }

        public virtual void Update(T entity)
        {
            _configuration.LiteDB!.GetCollection<T>().Update(entity.Id, entity);
        }

        public virtual IEnumerable<T> Take(int amount, bool takeLast = true)
        {
            var collection = _configuration.LiteDB!.GetCollection<T>().FindAll();

            if (takeLast)
                return collection.TakeLast(amount).Reverse();

            return collection.Take(amount);
        }


    }
}
