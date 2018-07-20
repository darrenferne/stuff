using AutoMapper;
using BWF.DataServices.Core.Abstract;
using BWF.DataServices.Metadata.Attributes.Actions;
using SchemaBrowser.Domain;

namespace SchemaBrowser.DataService
{
    [ViewAction("Db Schema")]
    public class DbSchemaRecordType : RecordType<DbSchema, DbSchema>
    {
        public override void ConfigureMapper()
        {
            Mapper.CreateMap<DbSchema, DbSchema>();
        }
    }
}
