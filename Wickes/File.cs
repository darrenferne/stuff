using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wickes
{
    public class File : ComponentBase
    {
        public File(string sourcePath = "", string name = "", string description = "")
            : base(name, description)
        {
            SourcePath = sourcePath;
        }

        public Media LocationMedia { get; set; }
        public string SourcePath { get; set; }
        public string KeyPath { get; set; }
    }
}
