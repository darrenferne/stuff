using Brady.Trade.DataService.Core.Interfaces;
using Brady.Trade.Domain.Interfaces;
using BWF.DataServices.Core.Abstract;
using BWF.DataServices.Core.Interfaces;
using BWF.DataServices.QueryConverters.InMemory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Repository.InMemory
{
    public class InMemoryTradeDataServiceRepository : InMemoryDataServiceRepository, IInMemoryTradeDataServiceRepository
    {
        ConcurrentDictionary<Type, long> _typeNextIds;
        ConcurrentDictionary<Type, ConcurrentDictionary<long, IHaveAssignableId<long>>> _typeRepositories;

        public InMemoryTradeDataServiceRepository(IAuthorisation authorisation, IMetadataProvider metadataProvider)
            : base("curveprovider", authorisation, metadataProvider)
        {
            _typeNextIds = new ConcurrentDictionary<Type, long>();
            _typeRepositories = new ConcurrentDictionary<Type, ConcurrentDictionary<long, IHaveAssignableId<long>>>();
        }

        private ConcurrentDictionary<long, IHaveAssignableId<long>> GetRepository<T_item>()
        {
            return GetRepository(typeof(T_item));
        }

        private Type GetBaseType(Type itemType)
        {
            if (itemType.BaseType != itemType && itemType.BaseType != typeof(object))
                return GetBaseType(itemType.BaseType);
            else
                return itemType;
        }

        private ConcurrentDictionary<long, IHaveAssignableId<long>> GetRepository(Type itemType)
        {
            var baseType = GetBaseType(itemType);
            var repository = _typeRepositories.GetOrAdd(baseType, new ConcurrentDictionary<long, IHaveAssignableId<long>>());

            return repository;
        }


        public T_item Create<T_item>(T_item item) where T_item : class, IHaveAssignableId<long>
        {
            Type itemType = GetBaseType(typeof(T_item));
            var repository = GetRepository(itemType);

            item.Id = _typeNextIds.AddOrUpdate(itemType, 1, (i, id) => id + 1);

            repository.TryAdd(item.Id, item);

            return item;
        }

        public void Delete<T_item>(long id) where T_item : class, IHaveAssignableId<long>
        {
            var repository = GetRepository<T_item>();

            IHaveAssignableId<long> deleted;
            repository.TryRemove(id, out deleted);
        }

        public T_item Get<T_item>(long id) where T_item : class, IHaveAssignableId<long>
        {
            var repository = GetRepository<T_item>();
            var result = (T_item)repository.Values.Where(i => i.Id == id).SingleOrDefault();
            return result;
        }

        public override IEnumerable<T> Get<T>()
        {
            var repository = GetRepository<T>();
            var result = repository.Values.Where(i => i is T).Select(i => (T)i).ToList();
            return result;
        }

        public override IEnumerable<long> GetRoleIdsThatHaveServiceLevelPermission(string permissionType, string permissionName)
        {
            return Enumerable.Empty<long>();
        }

        public void Update<T_item>(T_item item) where T_item : class, IHaveAssignableId<long>
        {
            var repository = GetRepository<T_item>();

            repository.TryUpdate(item.Id, item, null);
        }

        protected override IQueryExecutor GetQueryExecutor(string username, string token)
        {
            return new InMemoryQueryExecutor(this);
        }

        public IEnumerable<T_item> GetWhere<T_item>(Predicate<T_item> filter) where T_item : class, IHaveAssignableId<long>
        {
            var repository = GetRepository<T_item>();
            var result = repository.Values.Where(i => i is T_item && filter((T_item)i)).Select(i => (T_item)i).ToList();
            return result;
        }
    }
}
