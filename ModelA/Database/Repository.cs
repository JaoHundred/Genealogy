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

        public T FindById(long id)
        {
            return _configuration.LiteDB!.GetCollection<T>().FindById(id);
        }

        public void Upsert(T entity)
        {

            _configuration.LiteDB!.GetCollection<T>().Upsert(entity);
            //var obj = FindById(entity.Id);

            //if (obj == null)
            //    _configuration.LiteDB.GetCollection<T>().Insert(entity.Id, entity);
            //else
            //    Update(entity);
        }

        public void Delete(T entity)
        {
            _configuration.LiteDB!.GetCollection<T>().Delete(entity.Id);
        }

        public void Update(T entity)
        {
            _configuration.LiteDB!.GetCollection<T>().Update(entity.Id, entity);
        }

        public IEnumerable<T> Take(int amount)
        {
            return _configuration.LiteDB!.GetCollection<T>().FindAll().Take(amount);
        }
    }
}
