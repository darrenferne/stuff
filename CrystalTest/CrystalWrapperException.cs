using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
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
