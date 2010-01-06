using System;

namespace leetreveil.AutoUpdate.Core.Zip
{
    public class EmptyZipFileException : Exception
    {
        public EmptyZipFileException(){}
        public EmptyZipFileException(string message) : base(message){}
    }
}