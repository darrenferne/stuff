using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wickes
{
    public abstract class ComponentBase
    {
        public ComponentBase(string name = "", string description = "")
        {
            Id = new Guid();

            Name = name;
            Description = description;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
