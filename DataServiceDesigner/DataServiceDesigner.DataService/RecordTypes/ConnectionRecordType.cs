using System;
using AutoMapper;
using BWF.DataServices.Core.Concrete.ChangeSets;
using BWF.DataServices.Domain.Models;
using BWF.DataServices.Metadata.Attributes.Actions;
using BWF.DataServices.Support.NHibernate.Abstract;
using DataServiceDesigner.Domain;

namespace DataServiceDesigner.DataService
{
    [CreateAction("Create")]
    [EditAction("Edit")]
    [DeleteAction("Delete")]
    public class ConnectionRecordType : ChangeableRecordType<DesignerConnection, long, ConnectionBatchValidator>
    {
        //ISchemaRepository _repository;

        //public ConnectionRecordType(ISchemaRepository repository)
        public ConnectionRecordType()
        {
        //    _repository = repository;
        }

        public override void ConfigureMapper()
        {
            Mapper.CreateMap<DesignerConnection, DesignerConnection>();
        }

        public override Action<ChangeSet<long, DesignerConnection>, BatchSaveContext<long, DesignerConnection>, string> PostSaveAction
        {
            get { return DoPostSaveAction; }
        }

        private void DoPostSaveAction(ChangeSet<long, DesignerConnection> changeSet, BatchSaveContext<long, DesignerConnection> context, string noIdeaWhatThisIs)
        {
            //foreach (var newRec in changeSet.Create)
            //{
            //    _repository.FetchSchema(newRec.Value);
            //}

            //foreach (var newRec in changeSet.Update)
            //{
            //    var databaseType = newRec.Value.DatabaseType;
            //    var connectionString = newRec.Value.ConnectionString;

            //    _repository.FetchSchema(newRec.Value, true);
            //}
        }
    }
}
