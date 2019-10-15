using System;
using AutoMapper;
using BWF.DataServices.Core.Concrete.ChangeSets;
using BWF.DataServices.Domain.Models;
using BWF.DataServices.Metadata.Attributes.Actions;
using DataServiceDesigner.Domain;

namespace DataServiceDesigner.DataService
{
    [CreateAction("Create")]
    [EditAction("Edit")]
    [DeleteAction("Delete")]
    public class DataServiceConnectionRecordType : ObservableRecordType<DataServiceConnection, long, DataServiceConnectionBatchValidator>
    {
        ISchemaBrowserHelpers _connectionManager;
        public DataServiceConnectionRecordType(ISchemaBrowserHelpers connectionManager)
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
