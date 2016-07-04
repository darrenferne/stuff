using AutoMapper;
using Brady.Trade.Domain.Interfaces;
using Brady.Trade.Repository;
using BWF.DataServices.Core.Abstract;
using BWF.DataServices.Core.Concrete.ChangeSets;
using BWF.DataServices.Core.Interfaces;
using BWF.DataServices.Core.Interfaces.ChangeSets;
using BWF.DataServices.Domain.Interfaces;
using BWF.DataServices.Domain.Models;
using BWF.DataServices.Metadata.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.DataService.RecordTypes
{
    public class InMemoryRecordType<T_record, T_validator> 
        : RecordType<T_record>, IChangeableRecordType<long, T_record>, ISupportChangeSets
        where T_record : class, IHaveAssignableId<long>
        where T_validator : IValidator<T_record>, new()
    {
        protected Type _idType = typeof(long);
        protected Type _itemType = typeof(T_record);
        protected IMetadataProvider _metadataProvider;
        protected IInMemoryTradeDataServiceRepository _repository;

        public InMemoryRecordType(IMetadataProvider metadataProvider, IInMemoryTradeDataServiceRepository repository)
            : base()
        {
            _metadataProvider = metadataProvider;
            _repository = repository;
        }

        public Type IdType
        {
            get { return _idType; }
        }

        public Type ItemType
        {
            get { return _itemType; }
        }

        public override void ConfigureMapper()
        {
            Mapper.CreateMap<T_record, T_record>();
        }

        public IChangeSet GetNewChangeSet()
        {
            return new ChangeSet<long, T_record>();
        }

        public virtual ChangeSetResult<long> ProcessChangeSet(IDataService dataService, ChangeSet<long, T_record> changeSet, ProcessChangeSetSettings settings)
        {
            var result = new ChangeSetResult<long>();

            foreach (var deletion in changeSet.Delete)
            {
                _repository.Delete<T_record>(deletion);

                result.SuccessfullyDeleted.Add(deletion);
            }

            var validator = new T_validator();
            foreach (var update in changeSet.Update)
            {
                var validationResult = validator.Validate(update.Value);
                if (validationResult.IsValid)
                {
                    try
                    {
                        _repository.Update<T_record>(update.Value);

                        result.SuccessfullyUpdated.Add(update.Key);
                        result.SuccessfullyUpdatedItems.Add(update.Key, update.Value);
                    }
                    catch (Exception ex)
                    {
                        result.FailedUpdates.Add(update.Key, new MessageSet(ex.Message));
                    }
                }
                else
                {
                    var modelValidation = new ModelValidation();
                    validationResult.Errors.ToList().ForEach(x =>
                    {
                        if (string.IsNullOrWhiteSpace(x.PropertyName))
                            modelValidation.ModelValidations.Add(x.ErrorMessage);
                        else
                            modelValidation.PropertyValidations.Add(x.PropertyName, x.ErrorMessage);
                    });
                    result.FailedUpdates.Add(update.Key, modelValidation);
                }
            }

            foreach (var create in changeSet.Create)
            {
                var validationResult = validator.Validate(create.Value);
                if (validationResult.IsValid)
                {
                    try
                    {
                        var created = _repository.Create<T_record>(create.Value);

                        result.SuccessfullyCreated.Add(create.Key, created.Id);
                        result.SuccessfullyCreatedItems.Add(created.Id, created);
                    }
                    catch (Exception ex)
                    {
                        result.FailedCreates.Add(create.Key, new MessageSet(ex.Message));
                    }
                }
                else
                {
                    var modelValidation = new ModelValidation();
                    validationResult.Errors.ToList().ForEach(x =>
                    {
                        if (string.IsNullOrWhiteSpace(x.PropertyName))
                            modelValidation.ModelValidations.Add(x.ErrorMessage);
                        else
                            modelValidation.PropertyValidations.Add(x.PropertyName, x.ErrorMessage);
                    });
                    result.FailedCreates.Add(create.Key, modelValidation);
                }
            }

            dataService.NotifyOfExternalChangesToData();

            return result;
        }

        public ChangeSetResult<long> ProcessChangeSet(IDataService dataService, string token, string username, ChangeSet<long, T_record> changeSet, bool persistChanges)
        {
            var settings = new ProcessChangeSetSettings(token, username, persistChanges);
            return ProcessChangeSet(dataService, changeSet, settings);
        }
    }
}
