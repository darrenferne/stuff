using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BWF.DataServices.Core.Concrete.ChangeSets;
using BWF.DataServices.Core.Interfaces.ChangeSets;
using BWF.DataServices.Domain.Interfaces;
using BWF.DataServices.Domain.Models;
using DataServiceDesigner.Domain;
using Ninject;
using SchemaBrowser.DataService;
using SchemaBrowser.Domain;

namespace DataServiceDesigner.DataService
{
    public class SchemaBrowerHelpers : ISchemaBrowserHelpers
    {
        IKernel _kernel;
        ISchemaBrowserRepository _sbRepository;
        ISchemaBrowserDataService _sbDataService;
        IChangeableRecordType<long, DbConnection> _dbConnectionRecordType;
        public SchemaBrowerHelpers(IKernel kernel)
        {
            _kernel = kernel;
        }

        bool Initialise()
        {
            if (_sbDataService is null)
            {
                try
                {
                    _sbRepository = _kernel.Get<ISchemaBrowserRepository>();
                    _sbDataService = _kernel.Get<ISchemaBrowserDataService>();
                    _dbConnectionRecordType = _sbDataService.GetRecordType(nameof(DbConnection)) as IChangeableRecordType<long, DbConnection>;
                }
                catch
                {
                    _sbDataService = null;
                    _dbConnectionRecordType = null;
                }
            }
            return !(_sbDataService is null);
        }

        public void SynchConnections(IChangeSet changeSet, string token)
        {
            if (Initialise())
            {
                var dsConnectionChangeSet = changeSet as ChangeSet<long, DataServiceConnection>;
                var sbConnectionChangeSet = _dbConnectionRecordType.GetNewChangeSet() as ChangeSet<long, DbConnection>;

                var settings = new ProcessChangeSetSettings(token);
                var itemRef = 1L;

                foreach (DataServiceConnection dsConnection in dsConnectionChangeSet.Create.Values)
                {
                    var sbConnection = new DbConnection
                    {
                        ExternalId = dsConnection.Id,
                        Name = dsConnection.Name,
                        DatabaseType = dsConnection.DatabaseType,
                        DataSource = dsConnection.DataSource,
                        InitialCatalog = dsConnection.InitialCatalog,
                        Username = dsConnection.Username,
                        Password = dsConnection.Password,
                        UseIntegratedSecurity = dsConnection.UseIntegratedSecurity,
                        ConnectionString = dsConnection.ConnectionString
                    };
                    sbConnectionChangeSet.AddCreate(itemRef, sbConnection, Enumerable.Empty<long>(), Enumerable.Empty<long>());
                    itemRef++;
                }

                _dbConnectionRecordType.ProcessChangeSet(_sbDataService, sbConnectionChangeSet, settings);
            }
        }

        private string ToDisplayName(string name, bool pluralise = false)
        {
            StringBuilder displayName = new StringBuilder();
            displayName.Append( char.ToUpper(name[0]));
            for (int i = 1; i < name.Length; i++)
            {
                if (char.IsUpper(name[i]))
                {
                    displayName.Append(" ");
                    displayName.Append(name[i]);
                }
                else 
                {
                    displayName.Append(name[i]);
                }
            }
            if (pluralise)
            {
                if (name.ToLower().EndsWith("ty"))
                {
                    displayName.Remove(displayName.Length - 1, 1);
                    displayName.Append("ties");
                }
                else
                {
                    displayName.Append("s");
                }
            }

            return displayName.ToString();
        }

        public void AddDefaultPropertiesToObject(DomainObject domainObject)
        {
            var dbObjectProperties = _sbRepository.GetWhere<DbObjectProperty>(p => p.ObjectName == domainObject.TableName && p.SchemaName == domainObject.Schema.SchemaName);
            var primaryKey = _sbRepository.GetWhere<DbObjectIndex>(p => p.SchemaName == domainObject.Schema.SchemaName && p.TableName == domainObject.TableName).FirstOrDefault();
            
            var domainObjectProperties = dbObjectProperties.Select(r => new DomainObjectProperty()
            {
                Object = domainObject,
                ColumnName = r.Name,
                ColumnType = r.ColumnType,
                PropertyName = r.Name,
                DisplayName = ToDisplayName(r.Name),
                PropertyType = (PropertyType)Enum.Parse(typeof(PropertyType), r.NetType, true),
                Length = r.ColumnLength,
                IsNullable = r.IsNullable,
                IsPartOfKey = primaryKey?.Columns?.Contains(r.Name) ?? false,
                IncludeInDefaultView = true
            });
            domainObject.Properties = new List<DomainObjectProperty>(domainObjectProperties);
        }

        public void AddDefaultObjectsToSchema(DomainSchema schema)
        {
            var dbObjects = _sbRepository.GetWhere<DbObject>(o => o.SchemaName == schema.SchemaName);

            var domainObjects = dbObjects.Select(r =>
            {
                var domainObject = new DomainObject()
                {
                    Schema = schema,
                    TableName = r.Name,
                    ObjectName = r.Name,
                    DisplayName = ToDisplayName(r.Name),
                    PluralisedDisplayName = ToDisplayName(r.Name, true)
                };
                AddDefaultPropertiesToObject(domainObject);
                return domainObject;
            });

            schema.Objects = new List<DomainObject>(domainObjects);
        }
    }
}
