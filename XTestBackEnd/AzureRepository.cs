using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADOORM;
using Microsoft.Extensions.Logging;
using XTestBackEnd.Interfaces;

namespace XTestBackEnd
{
    public class AzureRepository<T>: IRepository<T> where T : class
    {
        private readonly ORMLight<T> ormLight;
        public AzureRepository(string connectionString, ILogger logger)
        {
            ormLight = new ORMLight<T>(connectionString, Constants.baseName);
        }

        public void Create(T item)
        {
            ormLight.ObjectSave(item);
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public T GetById(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetList()
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }
        public void Update(T item)
        {
            throw new NotImplementedException();
        }
    }
}
