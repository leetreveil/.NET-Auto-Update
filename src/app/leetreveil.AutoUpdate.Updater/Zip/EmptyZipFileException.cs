using System;

namespace leetreveil.AutoUpdate.Updater.Zip
{
    public class EmptyZipFileException : Exception
    {
        public EmptyZipFileException(){}
        public EmptyZipFileException(string message) : base(message){}
    }
}