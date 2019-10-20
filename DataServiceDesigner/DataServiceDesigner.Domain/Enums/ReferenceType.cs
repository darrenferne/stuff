using BWF.DataServices.Metadata.Enums;
using BWF.Enums.Attributes;
using Newtonsoft.Json;

namespace DataServiceDesigner.Domain
{
    [JsonConverter(typeof(RichEnumConverter))]
    public enum ReferenceType
    {
        [RichEnum("OneToMany", "OneToMany")]
        OneToMany,
        [RichEnum("ManyToMany", "ManyToMany")]
        ManyToMany
    }
}
