using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contracts.Interfaces
{
    public interface IRepository<T> : IDisposable where T : class
    {
        IEnumerable<T> GetList();
        T GetById(int id);
        List<T> GetByFieldValue(string fieldName, string value, bool isNumber);
        void Create(T item);
        void Update(T item);
        void Delete(int id);
    }
}
