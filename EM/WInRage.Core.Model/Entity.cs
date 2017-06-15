using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WInRage.Core.Model
{
    public class Entity
    {
        IList<IEntityProperty> _properties;
        IList<IEntityAttribute> _attributes;

        public Entity()
        {
            _properties = new List<IEntityProperty>();
            _attributes = new List<IEntityAttribute>();
        }

        public IList<IEntityProperty> Properties
        {
            get { return _properties; }
        }

        public IList<IEntityAttribute> Attributes
        {
            get { return _attributes; }
        }

        public string Name { get; set; }
        public string Description { get; set; }
    }
}
