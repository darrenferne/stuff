using System;
using System.Runtime.Serialization;

namespace Brady.Limits.ActionProcessing.Core
{
    public class ActionProcessorException : Exception
    {
        public ActionProcessorException()
        { }

        public ActionProcessorException(string message) : base(message)
        { }

        public ActionProcessorException(string message, Exception innerException) : base(message, innerException)
        { }

        protected ActionProcessorException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
    }
}
