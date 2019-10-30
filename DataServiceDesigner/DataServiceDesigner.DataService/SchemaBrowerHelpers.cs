using System;
using System.Collections.Generic;
using System.Globalization;
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
            string displayName = string.Empty;
            if (name.Contains('_'))
            {
                var parts = name.ToLower().Split('_').Select(p => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(p));
                displayName = string.Join(" ", parts);
            }
            else
            {
                //if everything is upper case then lower case it, otherwise it's mixed or varied case which is fine
                if (name == name.ToUpper())
                    name = name.ToLower();

                var builder = new StringBuilder();
                builder.Append(char.ToUpper(name[0]));
                for (int i = 1; i < name.Length; i++)
                {
                    if (char.IsUpper(name[i]) && !char.IsUpper(name[i - 1]))
                    {
                        builder.Append(" ");
                        builder.Append(name[i]);
                    }
                    else
                    {
                        builder.Append(name[i]);
                    }
                }
                displayName = builder.ToString();
            }

            if (pluralise)
            {
                if (!name.ToLower().EndsWith("s"))
                {
                    if (name.ToLower().EndsWith("ty"))
                    {
                        displayName.Remove(displayName.Length - 1, 1);
                        displayName += "ties";
                    }
                    else
                    {
                        displayName += "s";
                    }
                }
            }
            
            return displayName;
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
                PropertyType = Enum.TryParse<PropertyType>(r.NetType, true, out var propertyType) ? propertyType : PropertyType.Undefined,
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

        public void AddDefaultRelationshipsToSchema(DomainSchema domainSchema)
        {
            var dbRelationships = _sbRepository.GetWhere<DbObjectForeignKey>(fk => fk.SchemaName == domainSchema.SchemaName);

            var domainReferences = dbRelationships.Select(r =>
            {
                var domainReference = new DomainObjectReference()
                {
                    Schema =domainSchema,
                    ReferenceName = r.ConstraintName,
                    ConstraintName = r.ConstraintName,
                    Child = domainSchema.Objects.SingleOrDefault(o => o.Schema.SchemaName == r.SchemaName && o.TableName == r.TableName),
                    Parent = domainSchema.Objects.SingleOrDefault(o => o.Schema.SchemaName == r.ReferencedIndex.SchemaName && o.TableName == r.ReferencedIndex.TableName),
                };

                domainReference.Properties = r.Columns.Select((rc, i) => new DomainObjectReferenceProperty()
                {
                    Reference = domainReference,
                    ChildProperty = domainReference.Child?.Properties.FirstOrDefault(cp => cp.ColumnName ==  rc),
                    ParentProperty = domainReference.Parent?.Properties.FirstOrDefault(cp => cp.ColumnName == r.ReferencedIndex.Columns[i])
                }).ToList();

                return domainReference;
            });

            //Exclude any reference that has a parent or child that was not found ot that has self referencing properties
            domainSchema.References = new List<DomainObjectReference>(domainReferences.Where(r => !(r.Parent is null || r.Child is null) && !r.Properties.Any(p => p.ParentProperty == p.ChildProperty)));
        }
    }
}
