using AutoMapper;
using BWF.DataServices.Core.Abstract;
using BWF.DataServices.Metadata.Attributes.Actions;
using SchemaBrowser.Domain;

namespace SchemaBrowser.DataService
{
    [ViewAction("Db Object Foreign Key")]
    public class DbObjectForeignKeyRecordType : RecordType<DbObjectForeignKey, DbObjectForeignKey>
    {
        public override void ConfigureMapper()
        {
            Mapper.CreateMap<DbObjectForeignKey, DbObjectForeignKey>();
        }
    }
}
