using System;
using AutoMapper;
using BWF.DataServices.Core.Concrete.ChangeSets;
using BWF.DataServices.Domain.Models;
using BWF.DataServices.Metadata.Attributes.Actions;
using BWF.DataServices.Support.NHibernate.Abstract;
using DataServiceDesigner.Domain;

namespace DataServiceDesigner.DataService
{
    [CreateAction("Connection")]
    [EditAction("Connection")]
    [DeleteAction("Connection")]
    public class DataServiceConnectionRecordType : ChangeableRecordType<DataServiceConnection, long, DataServiceConnectionBatchValidator>
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
