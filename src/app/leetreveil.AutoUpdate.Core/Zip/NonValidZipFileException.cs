using System;

namespace leetreveil.AutoUpdate.Core.Zip
{
    public class NonValidZipFileException : Exception
    {
        public NonValidZipFileException(string message): base(message){}
        public NonValidZipFileException(string message,Exception innerException) : base(message,innerException){}
    }
}