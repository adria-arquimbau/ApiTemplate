using System;

namespace ApiTemplate.Values.Domain.Exceptions
{
    public class KeyTooShortException : Exception
    {
        public string Key { get; }
        public int Length { get; }

        public string Type => "key-too-long";

        public KeyTooShortException(string message, string key, int length) : base(message)
        {
            Key = key;
            Length = length;
        }
    }
}
