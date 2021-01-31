using System;
using System.Runtime.Serialization;

namespace FirstAlert.Algorithm
{
    [Serializable]
    internal class ImageClassException : Exception
    {
        public ImageClassException()
        {
        }

        public ImageClassException(string message) : base(message)
        {
        }

        public ImageClassException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ImageClassException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}