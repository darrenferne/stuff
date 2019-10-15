using AutoMapper;
using BWF.DataServices.Core.Abstract;
using BWF.DataServices.Core.Concrete.ChangeSets;
using BWF.DataServices.Core.Interfaces;
using BWF.DataServices.Core.Interfaces.ChangeSets;
using BWF.DataServices.Domain.Interfaces;
using BWF.DataServices.Domain.Models;
using BWF.DataServices.Live.Notification;
using BWF.DataServices.Metadata.Attributes.Actions;
using BWF.DataServices.Metadata.Models;
using SchemaBrowser.Domain;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SchemaBrowser.DataService
{
    [CreateAction("Db Connection")]
    [EditAction("Db Connection")]
    [DeleteAction("Db Connection")]
    [ViewAction("Db Connection")]
    public class DbConnectionRecordType : RecordType<DbConnection, DbConnection>, IChangeableRecordType<long, DbConnection>, ISupportChangeSets
    {
        ISchemaBrowserRepository _repository;
        SchemaBrowserUtils _utils;

        public DbConnectionRecordType(ISchemaBrowserRepository repository)
            : base()
        {
            _repository = repository;
            _utils = new SchemaBrowserUtils();
        }
        public Type IdType => typeof(long);

        public Type ItemType => typeof(DbConnection);

        public override void ConfigureMapper()
        {
            Mapper.CreateMap<DbConnection, DbConnection>();
        }

        public IChangeSet GetNewChangeSet()
        {
            return new ChangeSet<long, DbConnection>();
        }

        public ChangeSetResult<long> ProcessChangeSet(IDataService dataService, string token, string username, ChangeSet<long, DbConnection> changeSet, bool persistChanges)
        {
            var settings = new ProcessChangeSetSettings(token, username, persistChanges);
            return ProcessChangeSet(dataService, changeSet, settings);
        }
    
        public ChangeSetResult<long> ProcessChangeSet(IDataService dataService, ChangeSet<long, DbConnection> changeSet, ProcessChangeSetSettings settings)
        {
            var result = new ChangeSetResult<long>();
            
            foreach (var deletion in changeSet.Delete)
            {
                _repository.Delete<DbConnection>(deletion);

                result.SuccessfullyDeleted.Add(deletion);
            }

            var validator = new DbConnectionValidator(_repository);
            foreach (var update in changeSet.Update)
            {
                var validationResult = validator.Validate(update.Value);
                if (validationResult.IsValid)
                {
                    try
                    {
                        update.Value.Status = DbConnectionStatus.ValidationPending;
                        update.Value.StatusMessage = string.Empty;

                        _repository.Update(update.Value);

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
                        create.Value.Status = DbConnectionStatus.ValidationPending;
                        create.Value.StatusMessage = string.Empty;

                        var created = _repository.Create(create.Value);
                        
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
            
            dataService.NotifyOfExternalChangesToData(new DataChangeEvent(dataService.DataServiceName, typeof(DbConnection).Name));

            PostProcessChangeSet(dataService, result);

            return result;
        }

        void PostProcessChangeSet(IDataService dataService, ChangeSetResult<long> changeSet)
        {
            string message = string.Empty;

            Task.Run(() =>
            {
                foreach (var deletedId in changeSet.SuccessfullyDeleted
                                            .Union(changeSet.SuccessfullyUpdatedItems.Select(u => ((DbConnection)u.Value).Id)))
                {
                    _repository.PurgeSchema(deletedId);

                    dataService.NotifyOfExternalChangesToData(new DataChangeEvent(dataService.DataServiceName, typeof(DbSchema).Name));
                    dataService.NotifyOfExternalChangesToData(new DataChangeEvent(dataService.DataServiceName, typeof(DbObject).Name));
                    dataService.NotifyOfExternalChangesToData(new DataChangeEvent(dataService.DataServiceName, typeof(DbObjectProperty).Name));
                }

                foreach (DbConnection connection in changeSet.SuccessfullyCreatedItems.Select(u => u.Value as DbConnection)
                                                    .Union(changeSet.SuccessfullyUpdatedItems.Select(u => u.Value as DbConnection)))
                {
                    connection.Status = DbConnectionStatus.ValidationPending;

                    _repository.Update(connection);
                    dataService.NotifyOfExternalChangesToData(new DataChangeEvent(dataService.DataServiceName, typeof(DbConnection).Name));

                    if (_utils.TestConnection(connection.DatabaseType, connection.ConnectionString, out message))
                        connection.Status = DbConnectionStatus.Valid;
                    else
                        connection.Status = DbConnectionStatus.Invalid;
                    connection.StatusMessage = message;

                    _repository.Update(connection);
                    dataService.NotifyOfExternalChangesToData(new DataChangeEvent(dataService.DataServiceName, typeof(DbConnection).Name));
                }

                foreach (DbConnection connection in changeSet.SuccessfullyCreatedItems.Select(u => u.Value as DbConnection)
                                                    .Union(changeSet.SuccessfullyUpdatedItems.Select(u => u.Value as DbConnection)))
                {
                    if (connection.Status == DbConnectionStatus.Valid)
                    {
                        connection.Status = DbConnectionStatus.SchemaPending;

                        _repository.Update(connection);
                        dataService.NotifyOfExternalChangesToData(new DataChangeEvent(dataService.DataServiceName, typeof(DbConnection).Name));

                        _repository.FetchSchema(connection);

                        dataService.NotifyOfExternalChangesToData(new DataChangeEvent(dataService.DataServiceName, typeof(DbSchema).Name));
                        dataService.NotifyOfExternalChangesToData(new DataChangeEvent(dataService.DataServiceName, typeof(DbObject).Name));
                        dataService.NotifyOfExternalChangesToData(new DataChangeEvent(dataService.DataServiceName, typeof(DbObjectProperty).Name));

                        connection.Status = DbConnectionStatus.SchemaAvailable;

                        _repository.Update(connection);
                        dataService.NotifyOfExternalChangesToData(new DataChangeEvent(dataService.DataServiceName, typeof(DbConnection).Name));
                    }
                }


                //    var dbObjects = _utils.GetDbObjects(created.DatabaseType, created.ConnectionString, false);
                //    var dbObjectPropertiess = _utils.GetDbObjectProperties(created.DatabaseType, created.ConnectionString);
                //    var dbSchemas = new Dictionary<string, DbSchema>();

                //    foreach (var dbObject in dbObjects)
                //    {
                //        if (!dbSchemas.ContainsKey(dbObject.SchemaName))
                //        {
                //            var schema = new DbSchema() { Name = dbObject.SchemaName, Objects = new List<DbObject>() };
                //            dbSchemas.Add(dbObject.SchemaName, schema);
                //        }

                //        dbSchemas[dbObject.SchemaName].Objects.Add(dbObject);

                //        _repository.Create<DbObject>(dbObject);

                //        foreach (var dbObjectProperty in dbObject.Properties)
                //        {
                //            _repository.Create<DbObjectProperty>(dbObjectProperty);
                //        }
                //    }

                //    foreach (var dbSchema in dbSchemas.Values)
                //    {
                //        _repository.Create<DbSchema>(dbSchema);
                //    }
                //}

                //        if (createOrUpdate)
                //    {
                //        dataService.NotifyOfExternalChangesToData(new DataChangeEvent(dataService.DataServiceName, typeof(DbSchema).Name));
                //        dataService.NotifyOfExternalChangesToData(new DataChangeEvent(dataService.DataServiceName, typeof(DbObject).Name));
                //        dataService.NotifyOfExternalChangesToData(new DataChangeEvent(dataService.DataServiceName, typeof(DbObjectProperty).Name));
                //    }
            });
        }
    }
}
