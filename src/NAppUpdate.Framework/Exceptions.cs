using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace NAppUpdate.Framework
{
    public class NAppUpdateException : Exception
    {
        public NAppUpdateException() : base() { }
        public NAppUpdateException(string message) : base(message) { }
        public NAppUpdateException(string message, Exception ex) : base(message, ex) { }
        public NAppUpdateException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    public class UpdateProcessFailedException : NAppUpdateException
    {
        public UpdateProcessFailedException() : base() { }
        public UpdateProcessFailedException(string message) : base(message) { }
        public UpdateProcessFailedException(string message, Exception ex) : base(message, ex) { }
        public UpdateProcessFailedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
