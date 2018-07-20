using BWF.DataServices.Core.Interfaces;
using SchemaBrowser.Domain;
using System;
using System.Collections.Generic;

namespace SchemaBrowser.DataService
{
    public interface ISchemaBrowserRepository : IDataServiceRepository
    {
        T Get<T>(long Id) where T : class, IHaveAssignableId<long>;
        IEnumerable<T> GetWhere<T>(Predicate<T> filter) where T : class, IHaveAssignableId<long>;
        T Create<T>(T item) where T : class, IHaveAssignableId<long>;
        void Update<T>(T item) where T : class, IHaveAssignableId<long>;
        void Delete<T>(long id) where T : class, IHaveAssignableId<long>;
        void DeleteWhere<T>(Predicate<T> filter) where T : class, IHaveAssignableId<long>;
        void PurgeSchema(long connectionId);
        void FetchSchema(DbConnection connection);
    }
}