using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADOORM;
using Microsoft.Extensions.Logging;
using Contracts.Interfaces;

namespace XTestBackEnd
{
    public class AzureRepository<T> : IRepository<T> where T : class
    {
        private readonly ORMLight<T> ormLight;
        public AzureRepository(string connectionString, ILogger logger)
        {
            ormLight = new ORMLight<T>(connectionString, Constants.baseName);
        }
        public void Create(T item)
        {
            ormLight.Save(item);
        }
        public void Delete(int id)
        {
            ormLight.Delete(id);
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
        public T GetById(int id)
        {
            return ormLight.GetRecords(id);
        }
        public List<T> GetByFieldValue(string fieldName, string value, bool isNumber)
        {
            return ormLight.GetRecordsByValue(fieldName, value, isNumber);
        }
        public IEnumerable<T> GetList()
        {
            return ormLight.GetRecords();
        }
        public void Update(T item)
        {
            ormLight.Save(item);
        }
    }
}
