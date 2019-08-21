using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    [Guid("6042F158-680E-4AB4-A24C-5331EBE3D38C")]
    public class CrystalWrapperException : ApplicationException
    {
        public CrystalWrapperException()
            : base()
        {
        }

        public CrystalWrapperException(string message) : base(message)
        {
        }

        public CrystalWrapperException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CrystalWrapperException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
