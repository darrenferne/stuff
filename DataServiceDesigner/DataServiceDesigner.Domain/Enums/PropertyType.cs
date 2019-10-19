﻿using BWF.DataServices.Metadata.Enums;
using BWF.Enums.Attributes;
using Newtonsoft.Json;

namespace DataServiceDesigner.Domain
{
    [JsonConverter(typeof(RichEnumConverter))]
    public enum PropertyType
    {
        [RichEnum("Undefined", "Undefined")]
        Undefined,
        [RichEnum("Int32", "Int32")]
        Int32,
        [RichEnum("Int", "Int")]
        Int  = Int32,
        [RichEnum("Int64", "Int64")]
        Int64,
        [RichEnum("Long", "Long")]
        Long = Int64,
        [RichEnum("Float", "Float")]
        Float,
        [RichEnum("Double", "Double")]
        Double,
        [RichEnum("Decimal", "Decinal")]
        Decimal,
        [RichEnum("DateTime", "DateTime")]
        DateTime,
        [RichEnum("DateTimeOffset", "DateTimeOffset")]
        DateTimeOffset,
        [RichEnum("Boolean", "Boolean")]
        Boolean,
        [RichEnum("String", "String")]
        String
    }
}