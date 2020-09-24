using System;

namespace ApiTemplate.Values.Domain.Exceptions
{
    public class KeyTooLongException : Exception
    {
        public string Key { get; }
        public int Length { get; }

        public string Type => "key-too-long";

        public KeyTooLongException(string message, string key, int length) : base(message)
        {
            Key = key;
            Length = length;
        }
    }
}
