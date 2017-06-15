using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WInRage.Core.Model
{
    public class Entity<T_model> : Entity where T_model : class
    {
        public Entity()
            : base()
        { }
    }
}
