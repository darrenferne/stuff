using AutoMapper;
using BWF.DataServices.Core.Abstract;
using BWF.DataServices.Metadata.Attributes.Actions;
using SchemaBrowser.Domain;

namespace SchemaBrowser.DataService
{
    [ViewAction("Db Object Property")]
    public class DbObjectPropertyRecordType : RecordType<DbObjectProperty, DbObjectProperty>
    {
        public override void ConfigureMapper()
        {
            Mapper.CreateMap<DbObjectProperty, DbObjectProperty>();
        }
    }
}
