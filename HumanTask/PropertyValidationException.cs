using System;
using System.Runtime.Serialization;

namespace HumanTask
{
    public class PropertyValidationException:Exception
    {
        public PropertyValidationException()
        {
        }

        public PropertyValidationException(string message) : base(message)
        {
        }

        public PropertyValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PropertyValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}