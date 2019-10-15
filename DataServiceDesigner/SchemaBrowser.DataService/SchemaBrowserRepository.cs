using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BWF.DataServices.Core.Abstract;
using BWF.DataServices.Core.Interfaces;
using BWF.DataServices.Domain.Models;
using BWF.DataServices.QueryConverters.InMemory;
using SchemaBrowser.Domain;

namespace SchemaBrowser.DataService
{
    public class SchemaBrowserRepository : InMemoryDataServiceRepository, ISchemaBrowserRepository
    {
        SchemaBrowserUtils _utils;

        ConcurrentDictionary<Type, long> _nextIds;
        ConcurrentDictionary<Type, ConcurrentDictionary<long, IHaveAssignableId<long>>> _repositories;

        public SchemaBrowserRepository(IAuthorisation authorisation, IMetadataProvider metadataProvider)
            : base(Constants.DataServiceName, authorisation, metadataProvider)
        {
            _utils = new SchemaBrowserUtils();
            _nextIds = new ConcurrentDictionary<Type, long>();
            _repositories = new ConcurrentDictionary<Type, ConcurrentDictionary<long, IHaveAssignableId<long>>>();
        }

        internal ConcurrentDictionary<long, IHaveAssignableId<long>> GetRepository<T>()
        {
            return GetRepository(typeof(T));
        }

        internal ConcurrentDictionary<long, IHaveAssignableId<long>> GetRepository(Type type)
        {
            if (!_repositories.ContainsKey(type))
                _repositories.TryAdd(type, new ConcurrentDictionary<long, IHaveAssignableId<long>>());

            return _repositories[type];
        }

        public void PurgeSchema(long connectionId)
        {
            DeleteWhere<DbObjectProperty>(p => p.Connection.Id == connectionId);
            DeleteWhere<DbObject>(p => p.Connection.Id == connectionId);
            DeleteWhere<DbSchema>(p => p.Connection.Id == connectionId);
        }

        public void FetchSchema(DbConnection connection)
        {
            var dbObjects = _utils.GetDbObjects(connection.DatabaseType, connection.ConnectionString, false);
            var dbObjectProperties = _utils.GetDbObjectProperties(connection.DatabaseType, connection.ConnectionString);
            var dbObjectPrimaryKeys = _utils.GetObjectPrimaryKeys(connection.DatabaseType, connection.ConnectionString);
            var dbSchemas = new Dictionary<string, DbSchema>();

            foreach (var dbObject in dbObjects)
            {
                dbObject.Connection = connection;
                Create<DbObject>(dbObject);

                if (!dbSchemas.ContainsKey(dbObject.SchemaName))
                {
                    var schema = new DbSchema() { Connection = connection, Name = dbObject.SchemaName/*, Objects = new List<DbObject>()*/ };
                    dbSchemas.Add(dbObject.SchemaName, schema);
                }
            }

            foreach (var dbObjectProperty in dbObjectProperties)
            {
                dbObjectProperty.Connection = connection;
                Create<DbObjectProperty>(dbObjectProperty);
            }

            foreach (var dbSchema in dbSchemas.Values)
            {
                Create<DbSchema>(dbSchema);
            }

            foreach (var dbObjectPrimaryKey in dbObjectPrimaryKeys)
            {
                dbObjectPrimaryKey.Connection = connection;
                Create<DbObjectPrimaryKey>(dbObjectPrimaryKey);
            }
        }

        public T Create<T>(T item) where T : class, IHaveAssignableId<long>
        {
            Type itemType = typeof(T);
            var repository = GetRepository(itemType);

            item.Id = _nextIds.AddOrUpdate(itemType, 1, (i, id) => id + 1);

            repository.TryAdd(item.Id, item);

            return item;
        }

        public void Update<T>(T item) where T : class, IHaveAssignableId<long>
        {
            var repository = GetRepository(typeof(T));
            IHaveAssignableId<long> currentValue;

            if (repository.TryGetValue(item.Id, out currentValue))
            {
                repository.TryUpdate(item.Id, item, currentValue);
            }
        }

        public void Delete<T>(long id) where T : class, IHaveAssignableId<long>
        {
            var repository = GetRepository<T>();

            IHaveAssignableId<long> deleted;
            repository.TryRemove(id, out deleted);
        }

        public void DeleteWhere<T>(Predicate<T> filter) where T : class, IHaveAssignableId<long>
        {
            var repository = GetRepository<T>();

            foreach (var item in repository.Values.Where(i => filter((T)i)))
                Delete<T>(item.Id);
        }

        public T Get<T>(long id) where T : class, IHaveAssignableId<long>
        {
            var repository = GetRepository<T>();
            var result = (T)repository.Values.Where(i => i.Id == id).SingleOrDefault();
            return result;
        }

        public IEnumerable<T> GetWhere<T>(Predicate<T> filter) where T : class, IHaveAssignableId<long>
        {
            var repository = GetRepository<T>();
            var result = repository.Values.Where(i => filter((T)i)).Select(i => (T)i).ToList();
            return result;
        }
        public override IEnumerable<T> Get<T>()
        {
            var repository = GetRepository<T>();
            var result = repository.Values.Where(i => i is T).Select(i => (T)i).ToList();
            return result;
        }

        protected override IQueryExecutor GetQueryExecutor(string username, string token)
        {
            return new InMemoryQueryExecutor(this);
        }

        public override IEnumerable<long> GetRoleIdsThatHaveServiceLevelPermission(string permissionType, string permissionName)
        {
            return Enumerable.Empty<long>();
        }

        public override void AddDataLevelPermissions(IEnumerable<BwfDataLevelPermission> dataLevelPermissions)
        {
            //Do nothing
        }

        public override void RemoveDataLevelPermissions(IEnumerable<BwfDataLevelPermission> dataLevelPermissions)
        {
            //Do nothing
        }
    }
}
