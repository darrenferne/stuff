using BWF.DataServices.Core.Concrete.ChangeSets;
using BWF.DataServices.Core.Interfaces.ChangeSets;
using BWF.DataServices.Core.Models;
using BWF.DataServices.Domain.Models;
using BWF.DataServices.Metadata.Interfaces;
using BWF.DataServices.Metadata.Models;
using BWF.DataServices.PortableClients;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Brady.Limits.PreliminaryContract.ActionProcessing.Tests
{
    public abstract class RecordTypeTestsBase<T> : ActionProcessingTestContainer
        where T : class, IHaveId<long>
    {
        IChangeableRecordType<long, T> _recordType;
        protected IChangeableRecordType<long, T> recordType
        {
            get
            {
                if (_recordType is null)
                    _recordType = base.recordTypes.First(r => r.TypeName == typeof(T).Name) as IChangeableRecordType<long, T>;
                return _recordType;
            }
        }
        
        public abstract T ConstructValidRecord();
        public abstract (T, IList<string>) ConstructInvalidRecord();
        public abstract T UpdateRecordWithValidDetails(T record);
        public abstract (T, IList<string>) UpdateRecordWithInvalidDetails(T record);

        public T CreateValidRecord()
        {
            var record = ConstructValidRecord();
            var changeSet = (ChangeSet<long, T>)recordType.GetNewChangeSet();
            var itemRef = 1L;
            
            changeSet.AddCreate(itemRef, record, Enumerable.Empty<long>(), Enumerable.Empty<long>());
            var processResult = recordType.ProcessChangeSet(ActionProcessingDataService, changeSet, new ProcessChangeSetSettings(username: MrAdmin.Name));

            return (T)processResult.SuccessfullyCreatedItems[processResult.SuccessfullyCreated[itemRef]];
        }

        [TestMethod]
        public virtual void Creating_a_new_valid_record_should_succeed()
        {
            var record = ConstructValidRecord();
            var changeSet = (ChangeSet<long, T>)recordType.GetNewChangeSet();
            var itemRef = 1L;
            
            changeSet.AddCreate(itemRef, record, Enumerable.Empty<long>(), Enumerable.Empty<long>());
            var processResult = recordType.ProcessChangeSet(ActionProcessingDataService, changeSet, new ProcessChangeSetSettings(username: MrAdmin.Name));

            assertOneSuccessfullyCreated(processResult, itemRef);
        }

        [TestMethod]
        public virtual void Creating_a_new_invalid_record_should_fail()
        {
            var (record, expectedErrors) = ConstructInvalidRecord();
            var changeSet = (ChangeSet<long, T>)recordType.GetNewChangeSet();
            var itemRef = 1L;
            
            changeSet.AddCreate(itemRef, record, Enumerable.Empty<long>(), Enumerable.Empty<long>());
            var processResult = recordType.ProcessChangeSet(ActionProcessingDataService, changeSet, new ProcessChangeSetSettings(username: MrAdmin.Name));

            assertOneFailedCreate(processResult, itemRef);

            var failure = processResult.FailedCreates[itemRef] as ModelValidation;
            Assert.AreEqual(expectedErrors.Count, failure.PropertyValidations.Count);
            foreach (var error in expectedErrors)
                Assert.IsTrue(failure.PropertyValidations.Values.Contains(error));
        }

        [TestMethod]
        public void Reading_existing_records_should_succeed()
        {
            var record = CreateValidRecord();

            var builder = new QueryBuilder<T>();
            var queryResult = ActionProcessingDataService.Query(new Query(builder.GetQuery()), MrAdmin.Name, AdminRoleIds, string.Empty, string.Empty, out var fault);

            Assert.AreEqual(1, queryResult.TotalCount);
        }

        [TestMethod]
        public void Updating_an_existing_record_with_valid_details_should_succeed()
        {
            var record = CreateValidRecord();
            var changeSet = (ChangeSet<long, T>)recordType.GetNewChangeSet();

            record = UpdateRecordWithValidDetails(record);
            
            changeSet.AddUpdate(record.Id, record);
            var processResult = recordType.ProcessChangeSet(ActionProcessingDataService, changeSet, new ProcessChangeSetSettings(username: MrAdmin.Name));

            assertOneSuccessfullyUpdated(processResult, record.Id);
        }

        [TestMethod]
        public void Updating_an_existing_record_with_invalid_details_should_fail()
        {
            var record = CreateValidRecord();
            var changeSet = (ChangeSet<long, T>)recordType.GetNewChangeSet();

            var  (updatedRecord, expectedErrors) = UpdateRecordWithInvalidDetails(record);
            
            changeSet.AddUpdate(record.Id, updatedRecord);
            var processResult = recordType.ProcessChangeSet(ActionProcessingDataService, changeSet, new ProcessChangeSetSettings(username: MrAdmin.Name));

            assertOneFailedUpdate(processResult, record.Id);

            var failure = processResult.FailedUpdates[record.Id] as ModelValidation;
            Assert.AreEqual(expectedErrors.Count, failure.PropertyValidations.Count);
            foreach (var error in expectedErrors)
                Assert.IsTrue(failure.PropertyValidations.Values.Contains(error));
        }

        [TestMethod]
        public void Deleting_an_existing_record_should_succeed()
        {
            var record = CreateValidRecord();
            var changeSet = (ChangeSet<long, T>)recordType.GetNewChangeSet();
            
            changeSet.AddDelete(record.Id);
            var processResult = recordType.ProcessChangeSet(ActionProcessingDataService, changeSet, new ProcessChangeSetSettings(username: MrAdmin.Name));

            assertOneSuccessfullyDeleted(processResult, record.Id);
        }
    }
}
