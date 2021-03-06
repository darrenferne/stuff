﻿using AutoMapper;
using BWF.DataServices.Core.Abstract;
using BWF.DataServices.Metadata.Attributes.Actions;
using SchemaBrowser.Domain;

namespace SchemaBrowser.DataService
{
    [ViewAction("Db Object Primary Key")]
    public class DbObjectPrimaryKeyRecordType : RecordType<DbObjectIndex, DbObjectIndex>
    {
        public override void ConfigureMapper()
        {
            Mapper.CreateMap<DbObjectIndex, DbObjectIndex>();
        }
    }
}
