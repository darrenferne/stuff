using AutoMapper;
using BWF.DataServices.Core.Abstract;
using BWF.DataServices.Metadata.Attributes.Actions;
using SchemaBrowser.Domain;

namespace SchemaBrowser.DataService
{
    [ViewAction("Db Object")]
    public class DbObjectRecordType : RecordType<DbObject, DbObject>
    {
        public override void ConfigureMapper()
        {
            Mapper.CreateMap<DbObject, DbObject>();
        }
    }
}
