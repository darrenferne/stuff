using System;
using System.Linq;
using AutoMapper;
using BWF.DataServices.Core.Concrete.ChangeSets;
using BWF.DataServices.Core.Interfaces.ChangeSets;
using BWF.DataServices.Domain.Models;
using BWF.DataServices.Metadata.Attributes.Actions;
using BWF.DataServices.Support.NHibernate.Abstract;
using DataServiceDesigner.Domain;
using SchemaBrowser.DataService;
using SchemaBrowser.Domain;

namespace DataServiceDesigner.DataService
{
    [CreateAction("Create")]
    [EditAction("Edit")]
    [DeleteAction("Delete")]
    public class DataServiceConnectionRecordType : ChangeableRecordType<DataServiceConnection, long, DataServiceConnectionBatchValidator>
    {
        ISchemaBrowserConnectionManager _connectionManager;
        public DataServiceConnectionRecordType(ISchemaBrowserConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public override void ConfigureMapper()
        {
            Mapper.CreateMap<DataServiceConnection, DataServiceConnection>();
        }

        public override Action<ChangeSet<long, DataServiceConnection>, BatchSaveContext<long, DataServiceConnection>, string> PostSavePostCommitAction
        {
            get
            {
                return (changeSet, ContextBoundObject, _) => 
                {
                    _connectionManager.SynchConnections(changeSet, ContextBoundObject.Token);
                };
            }
        }
    }
}
